<script setup lang="ts">
  import { ref } from 'vue';
  import { ChatService } from '@/services/chatService';
  import type { Message } from '@/types';

  const prompt = ref<HTMLTextAreaElement | null>(null);
  const promptWrapper = ref<HTMLDivElement | null>(null);
  const promptText = ref('');
  const conversation = ref<Message[]>([]);
  const chatService = new ChatService();

  async function handleSubmit() {
    // TODO: show the user that the LLM is thinking

    if (!promptText.value) {
      return;
    }

    promptText.value = '';

    conversation.value.push({
      role: 'user',
      content: [
        {
          type: 'text',
          text: promptText.value,
        },
      ],
    });

    try {
      const events = chatService.generateResponse(conversation.value);

      // TODO: Show the user the LLM is finished thinking

      for await (const event of events) {
        // TODO: Handle the event
        // 1. We want to display the text that the LLM is writing
        // as it is being received
        // 2. We need to scroll the messages container to the bottom
        // 3. We want to add the message complete to the conversation
        // 4. Reset the current message text
        console.log(event);
      }
    } catch (error) {
      if (error instanceof Error) {
        alert(error.message);
      }
    }
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
</script>

<template>
  <div class="container">
    <div class="messages-container"></div>
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

    .messages-container {
      flex: 1;
      overflow-y: auto;
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
</style>
