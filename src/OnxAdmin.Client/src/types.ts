export type EventData =
  | PingEventData
  | ErrorEventData
  | MessageStartEventData
  | MessageDeltaEventData
  | MessageStopEventData
  | ContentStartEventData
  | ContentDeltaEventData
  | ContentStopEventData
  | MessageCompleteEventData;

export type AddAttachmentEvent =
  | AddAttachmentCompleteEvent
  | AddAttachmentProgressEvent
  | AddAttachmentErrorEvent;

export type Message = {
  role: string;
  content: Content[];
};

export type Attachment = {
  id: string;
  uploadProgress: number;
  file: File;
};

type AddAttachmentCompleteEvent = {
  type: 'add_attachment_complete';
  id: string;
};

type AddAttachmentProgressEvent = {
  type: 'add_attachment_progress';
  progress: number;
};

type AddAttachmentErrorEvent = {
  type: 'add_attachment_error';
};

type PingEventData = {
  type: 'ping';
};

type ErrorEventData = {
  type: 'error';
  error: {
    type: string;
    message: string;
  };
};

type MessageStartEventData = {
  type: 'message_start';
  message: MessageResponse;
};

type MessageDeltaEventData = {
  type: 'message_delta';
  delta: {
    stop_reason: string | null;
    stop_sequence: string | null;
  };
  usage: AnthropicUsage;
};

type MessageStopEventData = {
  type: 'message_stop';
};

type ContentStartEventData = {
  type: 'content_block_start';
  index: number;
  content_block: Content;
};

type ContentDeltaEventData = {
  type: 'content_block_delta';
  index: number;
  delta: ContentDelta;
};

type ContentStopEventData = {
  type: 'content_block_stop';
  index: number;
};

type MessageCompleteEventData = {
  type: 'message_complete';
  message: MessageResponse;
  headers: AnthropicHeaders;
};

type ContentDelta = TextDelta | JsonDelta;

type TextDelta = {
  type: 'text_delta';
  text: string;
};

type JsonDelta = {
  type: 'input_json_delta';
  partial_json: string;
};

type MessageResponse = {
  id: string;
  model: string;
  role: string;
  stop_reason: string | null;
  stop_sequence: string | null;
  type: string;
  usage: AnthropicUsage;
  content: Content[];
};

type AnthropicHeaders = {
  requestId: string;
  rateLimitRequestsLimit: number;
  rateLimitRequestsRemaining: number;
  rateLimitRequestsReset: string;
  rateLimitTokensLimit: number;
  rateLimitTokensRemaining: number;
  rateLimitTokensReset: string;
  retryAfter: number;
};

type AnthropicUsage = {
  input_tokens: number;
  output_tokens: number;
};

type Content = TextContent | ImageContent | ToolUseContent | ToolResultContent;

type TextContent = {
  type: 'text';
  text: string;
};

type ImageContent = {
  type: 'image';
  source: {
    type: 'base64';
    mediaType: string;
    data: string;
  };
};

type ToolUseContent = {
  type: 'tool_use';
  id: string;
  name: string;
  input: Record<string, object>;
};

type ToolResultContent = {
  type: 'tool_result';
  toolUseId: string;
  content: string;
};
