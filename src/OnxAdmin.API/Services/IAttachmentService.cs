namespace OnxAdmin.API.Services;

interface IAttachmentService
{
  Task<Guid> AddAttachmentAsync(IFormFile file);
  Task<bool> RemoveAttachmentAsync(string attachmentId);
}