export type EventData = {
  type: string;
};

export type Message = {
  role: string;
  content: Content[];
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
