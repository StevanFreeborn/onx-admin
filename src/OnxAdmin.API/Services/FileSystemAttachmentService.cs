namespace OnxAdmin.API.Services;

public class FileSystemAttachmentService(
  IWebHostEnvironment env,
  IFileSystem fileSystem
) : IAttachmentService
{
  private const string AttachmentsDirectory = "Attachments";
  private readonly IWebHostEnvironment _env = env;
  private readonly IFileSystem _fileSystem = fileSystem;

  public async Task<Guid> AddAttachmentAsync(IFormFile attachment)
  {
    var attachmentId = Guid.NewGuid();
    var trustedFileName = _fileSystem.Path.GetRandomFileName();
    var fileExtension = _fileSystem.Path.GetExtension(attachment.FileName);
    var fileName = $"{trustedFileName}{fileExtension}";
    var directoryPath = _fileSystem.Path.Combine(_env.ContentRootPath, AttachmentsDirectory, attachmentId.ToString());
    var filePath = _fileSystem.Path.Combine(directoryPath, fileName);

    _fileSystem.Directory.CreateDirectory(directoryPath);
    await using var fileStream = _fileSystem.File.Create(filePath);
    await attachment.OpenReadStream().CopyToAsync(fileStream);
    return attachmentId;
  }

  public Task<bool> RemoveAttachmentAsync(string attachmentId)
  {
    try
    {
      var directoryPath = _fileSystem.Path.Combine(_env.ContentRootPath, AttachmentsDirectory, attachmentId);
      _fileSystem.Directory.Delete(directoryPath, true);
      return Task.FromResult(true);
    }
    catch (Exception)
    {
      return Task.FromResult(false);
    }
  }
}