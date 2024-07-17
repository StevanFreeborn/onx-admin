import type { Message, EventData } from '@/types';

export interface IChatService {
  generateResponse(conversation: Message[]): AsyncIterable<EventData>;
}

export class ChatService implements IChatService {
  async *generateResponse(conversation: Message[]): AsyncIterable<EventData> {
    const response = await fetch('/api/generate-response', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ conversation }),
    });

    if (response.ok === false) {
      throw new Error('Failed to generate response');
    }

    for await (const chunk of this.getStream(response)) {
      yield JSON.parse(chunk);
    }
  }

  private async *getStream(response: Response) {
    if (response.body === null) {
      return;
    }

    const reader = response.body.pipeThrough(new TextDecoderStream()).getReader();

    try {
      while (true) {
        const { done, value } = await reader.read();

        if (done) {
          break;
        }

        if (value === undefined) {
          continue;
        }

        yield value;
      }
    } finally {
      reader.releaseLock();
    }
  }
}
