namespace OnxAdmin.API.Services;

interface IAttachmentService
{
  Task<Guid> AddAttachmentAsync(IFormFile file);
  Task<bool> RemoveAttachmentAsync(string attachmentId);
  Task<string> GetAttachmentExtensionAsync(string attachmentId);
  Task<IEnumerable<string>> GetAttachmentLinesAsync(string attachmentId);
  Task<Attachment> GetAttachmentAsync(string attachmentId);
}