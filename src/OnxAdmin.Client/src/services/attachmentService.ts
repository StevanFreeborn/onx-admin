import type { AddAttachmentEvent } from '@/types';

export interface IAttachmentService {
  addAttachment: (attachment: File) => AsyncIterable<AddAttachmentEvent>;
  removeAttachment: (id: string) => Promise<boolean>;
}

export class AttachmentService implements IAttachmentService {
  async *addAttachment(attachment: File): AsyncIterable<AddAttachmentEvent> {
    let progress = 0;
    let response = null;
    let error = null;

    const formData = new FormData();
    formData.append('attachment', attachment);

    const request = new XMLHttpRequest();

    request.upload.onprogress = (e: ProgressEvent) => {
      progress = Math.ceil((e.loaded / e.total) * 100);
    };

    request.onreadystatechange = () => {
      if (request.readyState === 4) {
        if (request.status !== 200) {
          error = new Error('Failed to upload attachment');
          return;
        }

        response = JSON.parse(request.responseText);
      }
    };

    request.open('POST', '/api/attachments', true);
    request.send(formData);

    while (true) {
      if (error !== null) {
        yield { type: 'add_attachment_error' };
        break;
      }

      if (response !== null) {
        const res = response as { id: string };
        yield { type: 'add_attachment_complete', id: res.id };
        break;
      }

      yield { type: 'add_attachment_progress', progress };
      await new Promise(resolve => setTimeout(resolve, 100));
    }
  }

  async removeAttachment(id: string) {
    const response = await fetch(`/api/attachments/${id}`, {
      method: 'DELETE',
    });
    return response.ok;
  }
}
