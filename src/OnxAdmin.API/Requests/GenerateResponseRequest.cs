namespace OnxAdmin.API.Requests;

record GenerateResponseDto(string Message);

record GenerateResponseRequest(
  [FromBody] GenerateResponseDto GenerateResponseDto,
  [FromServices] IChatService ChatService
);