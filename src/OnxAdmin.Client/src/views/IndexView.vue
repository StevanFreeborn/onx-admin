<script setup lang="ts">
  import { computed, ref, nextTick, onMounted } from 'vue';
  import { ChatService } from '@/services/chatService';
  import type { Attachment, Message } from '@/types';
  import { marked } from 'marked';
  import { AttachmentService } from '@/services/attachmentService.js';

  const prompt = ref<HTMLTextAreaElement | null>(null);
  const promptWrapper = ref<HTMLDivElement | null>(null);
  const messagesContainer = ref<HTMLDivElement | null>(null);
  const fileInput = ref<HTMLInputElement | null>(null);
  const promptText = ref('');
  const llmResponse = ref('');
  const llmResponseHTML = computed(() => parseMarkdownToHTML(llmResponse.value));
  const isThinking = ref(false);
  const isUploading = ref(false);
  const conversation = ref<Message[]>([]);
  const conversationWithoutToolResults = computed(() =>
    conversation.value.filter(m => !m.content.some(c => c.type === 'tool_result'))
  );
  const attachments = ref<Attachment[]>([]);

  const chatService = new ChatService();
  const attachmentService = new AttachmentService();

  onMounted(() => {
    if (prompt.value === null) {
      return;
    }

    const resizeObserver = new ResizeObserver(entries => {
      for (const entry of entries) {
        if (entry.target === prompt.value) {
          resizePrompt();
        }
      }
    });

    resizeObserver.observe(prompt.value);
  });

  function parseMarkdownToHTML(text: string) {
    return marked.parse(text) as string;
  }

  function getToolResult(id: string) {
    let result = '';

    for (const message of conversation.value) {
      for (const content of message.content) {
        if (content.type === 'tool_result' && content.tool_use_id === id) {
          result = content.content;
          break;
        }
      }
    }

    return parseMarkdownToHTML(result);
  }

  function scrollToBottom() {
    if (messagesContainer.value === null) {
      return;
    }

    messagesContainer.value.scrollTo({
      top: messagesContainer.value.scrollHeight,
      behavior: 'smooth',
    });
  }

  function handlePromptFocus() {
    if (promptWrapper.value === null) {
      return;
    }

    // TODO: Styles will need to change based on the current theme
    promptWrapper.value.style.outline = '2px solid #ffffff';
  }

  function handlePromptBlur() {
    if (promptWrapper.value === null) {
      return;
    }

    promptWrapper.value.style.outline = 'none';
  }

  function resetPrompt() {
    if (prompt.value === null) {
      return;
    }

    prompt.value.style.height = 'auto';
    prompt.value.style.overflow = 'hidden';
    promptText.value = '';
    attachments.value = [];

    if (fileInput.value === null) {
      return;
    }

    fileInput.value.value = '';
  }

  function resizePrompt() {
    if (prompt.value === null) {
      return;
    }

    prompt.value.style.height = 'auto';

    const scrollHeight = prompt.value.scrollHeight;

    if (scrollHeight > 300) {
      prompt.value.style.height = '300px';
      prompt.value.style.overflow = 'auto';
      return;
    }

    prompt.value.style.height = `${scrollHeight}px`;
    prompt.value.style.overflow = 'hidden';
  }

  function handlePromptInput() {
    resizePrompt();
  }

  async function handleSubmit() {
    isThinking.value = true;

    if (!promptText.value) {
      return;
    }

    const userInput = `${promptText.value.trim()} ${attachments.value
      .map(a => `<p class="uploaded-attachment" data-attachment-id="${a.id}">${a.file.name}</p>`)
      .join('')}`;

    conversation.value.push({
      role: 'user',
      content: [
        {
          type: 'text',
          text: userInput,
        },
      ],
    });

    resetPrompt();
    await nextTick();
    scrollToBottom();

    let hasToolResult = false;

    do {
      try {
        isThinking.value = true;

        const events = chatService.generateResponse(conversation.value);

        for await (const event of events) {
          switch (event.type) {
            case 'message_start':
              isThinking.value = false;
              break;
            case 'content_block_delta':
              switch (event.delta.type) {
                case 'text_delta':
                  llmResponse.value += event.delta.text;
                  scrollToBottom();
                  break;
                default:
                  break;
              }
              break;
            case 'message_delta':
              isThinking.value = event.delta.stop_reason === 'tool_use';
              break;
            case 'message_complete':
              conversation.value.push({
                role: event.message.role,
                content: event.message.content,
              });
              llmResponse.value = '';
              break;
            case 'tool_result':
              conversation.value.push({
                role: event.message.role,
                content: event.message.content,
              });
              break;
            default:
              break;
          }
        }

        hasToolResult = conversation.value[conversation.value.length - 1].content.some(
          c => c.type === 'tool_result'
        );
      } catch (error) {
        isThinking.value = false;

        if (error instanceof Error) {
          alert(error.message);
        }
      }
    } while (hasToolResult);
  }

  async function handlePromptKeydown(e: KeyboardEvent) {
    if (e.key == 'Enter' && e.ctrlKey) {
      await handleSubmit();
    }
  }

  function handleAttachmentButtonClick() {
    if (fileInput.value === null) {
      return;
    }

    fileInput.value.click();
  }

  async function handleFileInputChange() {
    isUploading.value = true;

    try {
      if (fileInput.value === null || fileInput.value.files === null) {
        return;
      }

      const addFilePromises = Array.from(fileInput.value.files).map(async file => {
        attachments.value.push({
          id: '',
          uploadProgress: 0,
          file: file,
        });

        const events = attachmentService.addAttachment(file);

        for await (const e of events) {
          const attachment = attachments.value.find(a => a.file === file);

          switch (e.type) {
            case 'add_attachment_progress':
              if (attachment) {
                attachment.uploadProgress = e.progress;
              }
              break;
            case 'add_attachment_complete':
              if (attachment) {
                attachment.uploadProgress = 100;
                attachment.id = e.id;
              }
              break;
            default:
              if (attachment) {
                attachments.value.splice(attachments.value.indexOf(attachment), 1);
              }
              alert('Failed to add attachment');
              break;
          }
        }
      });

      fileInput.value.value = '';

      await Promise.all(addFilePromises);
    } finally {
      isUploading.value = false;
    }
  }

  async function handleRemoveFileButtonClick(id: string) {
    const result = await attachmentService.removeAttachment(id);

    if (result === false) {
      alert('Failed to remove attachment');
      return;
    }

    attachments.value = attachments.value.filter(a => a.id !== id);
  }
</script>

<template>
  <div class="container">
    <div class="messages-container" ref="messagesContainer">
      <div
        v-for="[index, message] in conversationWithoutToolResults.entries()"
        v-bind:key="index"
        :class="`message message-${message.role}`"
      >
        <div
          v-for="[index, content] in message.content.entries()"
          v-bind:key="index"
          class="content-container"
        >
          <div
            v-if="content.type === 'text'"
            class="message-wrapper"
            v-html="`${parseMarkdownToHTML(content.text)}`"
          ></div>
          <details v-if="content.type === 'tool_use'" class="tool">
            <summary class="tool-use">
              <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" fill="currentColor">
                <path
                  d="M352 320c88.4 0 160-71.6 160-160c0-15.3-2.2-30.1-6.2-44.2c-3.1-10.8-16.4-13.2-24.3-5.3l-76.8 76.8c-3 3-7.1 4.7-11.3 4.7H336c-8.8 0-16-7.2-16-16V118.6c0-4.2 1.7-8.3 4.7-11.3l76.8-76.8c7.9-7.9 5.4-21.2-5.3-24.3C382.1 2.2 367.3 0 352 0C263.6 0 192 71.6 192 160c0 19.1 3.4 37.5 9.5 54.5L19.9 396.1C7.2 408.8 0 426.1 0 444.1C0 481.6 30.4 512 67.9 512c18 0 35.3-7.2 48-19.9L297.5 310.5c17 6.2 35.4 9.5 54.5 9.5zM80 408a24 24 0 1 1 0 48 24 24 0 1 1 0-48z"
                />
              </svg>
              <p>{{ content.name }}</p>
            </summary>
            <div class="tool-result" v-html="getToolResult(content.id)"></div>
          </details>
        </div>
      </div>
      <div v-if="llmResponse.trim() !== ''" class="message message-assistant">
        <div class="content-container">
          <div class="message-wrapper" v-html="llmResponseHTML"></div>
        </div>
      </div>
      <div v-if="isThinking" class="message message-assistant">
        <div class="thinking">
          <span></span>
          <span></span>
          <span></span>
        </div>
      </div>
    </div>
    <!-- TODO: Support file uploads -->
    <div class="input-container">
      <div v-if="attachments.length > 0" class="attachment-wrapper">
        <div
          v-for="[index, attachment] in attachments.entries()"
          v-bind:key="index"
          class="attachment"
        >
          <div v-if="attachment.uploadProgress < 100" class="progress">
            <div class="bar" :style="{ width: `${attachment.uploadProgress}%` }"></div>
          </div>
          <div class="name-container">
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 384 512" fill="currentColor">
              <path
                d="M0 64C0 28.7 28.7 0 64 0H224V128c0 17.7 14.3 32 32 32H384V448c0 35.3-28.7 64-64 64H64c-35.3 0-64-28.7-64-64V64zm384 64H256V0L384 128z"
              />
            </svg>
            <p :title="`${attachment.file.name}`">{{ attachment.file.name }}</p>
          </div>
          <button
            v-if="attachment.uploadProgress === 100"
            type="button"
            @click="() => handleRemoveFileButtonClick(attachment.id)"
          >
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" fill="currentColor">
              <path
                d="M256 512A256 256 0 1 0 256 0a256 256 0 1 0 0 512zM175 175c9.4-9.4 24.6-9.4 33.9 0l47 47 47-47c9.4-9.4 24.6-9.4 33.9 0s9.4 24.6 0 33.9l-47 47 47 47c9.4 9.4 9.4 24.6 0 33.9s-24.6 9.4-33.9 0l-47-47-47 47c-9.4 9.4-24.6 9.4-33.9 0s-9.4-24.6 0-33.9l47-47-47-47c-9.4-9.4-9.4-24.6 0-33.9z"
              />
            </svg>
            <span class="sr-only">Remove Attachment</span>
          </button>
        </div>
      </div>
      <div ref="promptWrapper" class="prompt-wrapper">
        <label for="fileInput" class="sr-only">File Input</label>
        <input
          ref="fileInput"
          type="file"
          multiple
          style="display: none"
          id="fileInput"
          name="fileInput"
          @change="handleFileInputChange"
          :disabled="isThinking || isUploading"
        />
        <button
          class="attach-file-button"
          type="button"
          @click="handleAttachmentButtonClick"
          :disabled="isThinking || isUploading"
        >
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 448 512" fill="currentColor">
            <path
              d="M364.2 83.8c-24.4-24.4-64-24.4-88.4 0l-184 184c-42.1 42.1-42.1 110.3 0 152.4s110.3 42.1 152.4 0l152-152c10.9-10.9 28.7-10.9 39.6 0s10.9 28.7 0 39.6l-152 152c-64 64-167.6 64-231.6 0s-64-167.6 0-231.6l184-184c46.3-46.3 121.3-46.3 167.6 0s46.3 121.3 0 167.6l-176 176c-28.6 28.6-75 28.6-103.6 0s-28.6-75 0-103.6l144-144c10.9-10.9 28.7-10.9 39.6 0s10.9 28.7 0 39.6l-144 144c-6.7 6.7-6.7 17.7 0 24.4s17.7 6.7 24.4 0l176-176c24.4-24.4 24.4-64 0-88.4z"
            />
          </svg>
          <span class="sr-only">Attach File</span>
        </button>
        <label for="prompt" class="sr-only">Prompt</label>
        <textarea
          ref="prompt"
          name="prompt"
          id="prompt"
          placeholder="Message OnxAdmin"
          rows="1"
          dir="auto"
          v-model="promptText"
          @input="handlePromptInput"
          @focus="handlePromptFocus"
          @blur="handlePromptBlur"
          @keydown="handlePromptKeydown"
          :disabled="isThinking"
        ></textarea>
        <button
          type="button"
          class="send-button"
          @click="handleSubmit"
          :disabled="isThinking || isUploading"
        >
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" fill="currentColor">
            <path
              d="M498.1 5.6c10.1 7 15.4 19.1 13.5 31.2l-64 416c-1.5 9.7-7.4 18.2-16 23s-18.9 5.4-28 1.6L284 427.7l-68.5 74.1c-8.9 9.7-22.9 12.9-35.2 8.1S160 493.2 160 480V396.4c0-4 1.5-7.8 4.2-10.7L331.8 202.8c5.8-6.3 5.6-16-.4-22s-15.7-6.4-22-.7L106 360.8 17.7 316.6C7.1 311.3 .3 300.7 0 288.9s5.9-22.8 16.1-28.7l448-256c10.7-6.1 23.9-5.5 34 1.4z"
            />
          </svg>
          <span class="sr-only">Send</span>
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
  .container {
    --user-input-color: #2f2f2f;
    display: flex;
    flex-direction: column;
    height: 100%;
    width: 100%;
    max-width: 40rem;

    .messages-container {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
      flex: 1;
      overflow-y: auto;
      padding: 1.5rem 0.5rem;
      scrollbar-color: var(--secondary-black) var(--black);
      scroll-behavior: smooth;

      & .message {
        display: flex;
        flex-direction: column;
        gap: 0.5rem;

        &.message-user {
          align-items: flex-end;
          padding-left: 2rem;

          & .message-wrapper {
            background-color: var(--user-input-color);
            color: var(--white);
            padding: 0.5rem 1rem;
            border-radius: 0.5rem;
          }
        }

        &.message-assistant {
          align-items: flex-start;
          padding-right: 2rem;

          & .tool {
            display: flex;
            flex-direction: column;
            gap: 0.5rem;
            background-color: rgba(33, 150, 243, 0.6);
            padding: 0.5rem 1rem;
            border-radius: 0.5rem;

            & .tool-use {
              display: flex;
              align-items: center;
              gap: 0.5rem;

              & svg {
                width: 1rem;
                height: 1rem;
                flex-shrink: 0;
              }
            }

            & .tool-result {
              margin-left: 1rem;
              background-color: rgba(0, 128, 0, 0.6);
              color: var(--white);
              padding: 0.5rem 1rem;
              border-radius: 0.5rem;
            }
          }

          & .thinking {
            display: flex;
            align-items: center;
            gap: 0.5rem;

            & span {
              width: 0.5rem;
              height: 0.5rem;
              background-color: var(--white);
              border-radius: 50%;
              animation: thinking 1.5s infinite;
              animation-fill-mode: both;
            }

            & span:nth-child(2) {
              animation-delay: 0.2s;
            }

            & span:nth-child(3) {
              animation-delay: 0.4s;
            }
          }
        }

        .content-container {
          max-width: 100%;

          & .message-wrapper {
            display: flex;
            flex-direction: column;
            max-width: 100%;
            gap: 0.75rem;

            & a {
              color: #2196f3;
              text-decoration: none;
            }

            & a:hover {
              text-decoration: underline;
            }
          }
        }
      }
    }

    .input-container {
      display: flex;
      padding: 1rem;
      width: 100%;
      justify-content: center;
      flex-direction: column;
      gap: 0.25rem;

      & .attachment-wrapper {
        display: flex;
        align-items: flex-end;
        width: 100%;
        gap: 1rem;
        overflow-x: auto;
        scrollbar-color: var(--user-input-color) var(--black);
        scrollbar-width: thin;
        padding-top: 1rem;
        padding-bottom: 0.5rem;

        & .attachment {
          display: flex;
          flex-direction: column;
          gap: 0.5rem;
          padding: 0.5rem;
          background-color: var(--user-input-color);
          border-radius: 0.5rem;
          position: relative;
          max-width: 200px;
          height: max-content;

          .name-container {
            display: flex;
            align-items: center;
            gap: 0.5rem;

            & p {
              white-space: nowrap;
              overflow: hidden;
              text-overflow: ellipsis;
            }
          }

          .progress {
            background-color: var(--user-input-color);
            border-radius: 0.25rem;
            height: 0.25rem;
            width: 100%;

            & .bar {
              background-color: var(--background-color);
              border-radius: 0.25rem;
              height: 100%;
            }
          }

          & button {
            position: absolute;
            top: -1rem;
            right: -1rem;
            background-color: transparent;
            border: none;
            color: var(--white);
            cursor: pointer;
            padding: 0.5rem;
            display: none;
          }

          &:hover button {
            display: flex;
          }

          & svg {
            flex-shrink: 0;
            width: 1rem;
            height: 1rem;
          }
        }
      }

      & .prompt-wrapper {
        display: flex;
        align-items: flex-end;
        width: 100%;
        gap: 1rem;
        background-color: var(--user-input-color);
        padding: 1rem 2rem;
        border-radius: 0.5rem;

        & textarea {
          flex: 1;
          resize: none;
          background-color: var(--user-input-color);
          color: var(--white);
          border: none;
          font-size: 1rem;
          font-family: inherit;
          line-height: 1.5rem;
          height: 1.5rem;
          max-height: 100%;
          scrollbar-color: var(--user-input-color) var(--background-color);

          &:focus {
            outline: none;
          }
        }

        .send-button,
        .attach-file-button {
          color: var(--white);

          & svg {
            width: 1rem;
            height: 1rem;
          }
        }
      }
    }
  }

  @keyframes thinking {
    0% {
      opacity: 0.1;
    }

    20% {
      opacity: 1;
    }

    100% {
      opacity: 0.1;
    }
  }
</style>
