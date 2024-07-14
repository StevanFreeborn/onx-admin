using System.ComponentModel.DataAnnotations;

namespace OnxAdmin.API.Requests;

record GenerateResponseRequest(
  HttpContext Context,
  [FromBody] GenerateResponseDto GenerateResponseDto,
  [FromServices] IChatService ChatService
);

record GenerateResponseDto(List<Message> Conversation) : IValidatableObject
{
  public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
  {
    if (Conversation.Count == 0)
    {
      yield return new ValidationResult("Conversation must contain at least one message.", [nameof(Conversation)]);
    }
  }
}