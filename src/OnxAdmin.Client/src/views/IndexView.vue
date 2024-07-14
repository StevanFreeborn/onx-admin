<script setup lang="ts">
  import { computed, ref, nextTick } from 'vue';
  import { ChatService } from '@/services/chatService';
  import type { Message } from '@/types';
  import { marked } from 'marked';

  const prompt = ref<HTMLTextAreaElement | null>(null);
  const promptWrapper = ref<HTMLDivElement | null>(null);
  const messagesContainer = ref<HTMLDivElement | null>(null);
  const promptText = ref('');
  const llmResponse = ref('');
  const llmResponseHTML = computed(() => parseMarkdownToHTML(llmResponse.value));
  const isThinking = ref(false);
  const conversation = ref<Message[]>([]);
  const chatService = new ChatService();

  function parseMarkdownToHTML(text: string): string {
    return marked.parse(text);
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

    conversation.value.push({
      role: 'user',
      content: [
        {
          type: 'text',
          text: promptText.value,
        },
      ],
    });

    resetPrompt();
    await nextTick();
    scrollToBottom();

    try {
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
          case 'message_complete':
            conversation.value.push({
              role: event.message.role,
              content: event.message.content,
            });
            llmResponse.value = '';
            break;
          default:
            break;
        }
      }
    } catch (error) {
      isThinking.value = false;

      if (error instanceof Error) {
        alert(error.message);
      }
    }
  }
</script>

<template>
  <div class="container">
    <div class="messages-container" ref="messagesContainer">
      <div
        v-for="[index, message] in conversation.entries()"
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
          <div v-if="content.type === 'tool_use'" class="tool-use">
            <!-- TODO: Need to present any corresponding tool result -->
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" fill="currentColor">
              <path
                d="M352 320c88.4 0 160-71.6 160-160c0-15.3-2.2-30.1-6.2-44.2c-3.1-10.8-16.4-13.2-24.3-5.3l-76.8 76.8c-3 3-7.1 4.7-11.3 4.7H336c-8.8 0-16-7.2-16-16V118.6c0-4.2 1.7-8.3 4.7-11.3l76.8-76.8c7.9-7.9 5.4-21.2-5.3-24.3C382.1 2.2 367.3 0 352 0C263.6 0 192 71.6 192 160c0 19.1 3.4 37.5 9.5 54.5L19.9 396.1C7.2 408.8 0 426.1 0 444.1C0 481.6 30.4 512 67.9 512c18 0 35.3-7.2 48-19.9L297.5 310.5c17 6.2 35.4 9.5 54.5 9.5zM80 408a24 24 0 1 1 0 48 24 24 0 1 1 0-48z"
              />
            </svg>
            <p>{{ content.name }}</p>
          </div>
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
    <div class="input-container">
      <div ref="promptWrapper" class="prompt-wrapper">
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
        ></textarea>
        <button type="button" class="send-button" @click="handleSubmit">
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
            background-color: #2f2f2f;
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
              gap: 0.25rem;

              & svg {
                width: 1rem;
                height: 1rem;
              }
            }

            & .tool-result {
              margin-left: 1rem;
              background-color: rgba(0, 128, 0, 0.6);
              color: var(--white);
              padding: 0.5rem 1rem;
              border-radius: 0.5rem;
            }

            & .tool-result.error {
              background-color: rgba(255, 0, 0, 0.6);
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

      & .prompt-wrapper {
        display: flex;
        align-items: flex-end;
        width: 100%;
        gap: 1rem;
        background-color: #2f2f2f;
        padding: 1rem 2rem;
        border-radius: 0.5rem;

        & textarea {
          flex: 1;
          resize: none;
          background-color: #2f2f2f;
          color: var(--white);
          border: none;
          font-size: 1rem;
          font-family: inherit;
          line-height: 1.5rem;
          height: 1.5rem;
          max-height: 100%;
          scrollbar-color: #2f2f2f var(--background-color);

          &:focus {
            outline: none;
          }
        }

        .send-button {
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
