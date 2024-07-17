using Microsoft.AspNetCore.StaticFiles;

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

  public Task<string> GetAttachmentExtensionAsync(string attachmentId)
  {
    var directoryPath = _fileSystem.Path.Combine(_env.ContentRootPath, AttachmentsDirectory, attachmentId);
    var filePath = _fileSystem.Directory.GetFiles(directoryPath).First();
    var extension = _fileSystem.Path.GetExtension(filePath);
    return Task.FromResult(extension);
  }

  public async Task<IEnumerable<string>> GetAttachmentLinesAsync(string attachmentId)
  {
    var directoryPath = _fileSystem.Path.Combine(_env.ContentRootPath, AttachmentsDirectory, attachmentId);
    var filePath = _fileSystem.Directory.GetFiles(directoryPath).First();
    return await _fileSystem.File.ReadAllLinesAsync(filePath);
  }

  public async Task<Attachment> GetAttachmentAsync(string attachmentId)
  {
    var directoryPath = _fileSystem.Path.Combine(_env.ContentRootPath, AttachmentsDirectory, attachmentId);
    var filePath = _fileSystem.Directory.GetFiles(directoryPath).First();
    var fileName = _fileSystem.Path.GetFileName(filePath);

    var provider = new FileExtensionContentTypeProvider();

    if (provider.TryGetContentType(filePath, out string? contentType) is false)
    {
      contentType = "application/octet-stream";
    }

    var fileStream = _fileSystem.File.OpenRead(filePath);
    var memoryStream = new MemoryStream();
    await fileStream.CopyToAsync(memoryStream);
    memoryStream.Position = 0;
    return new(attachmentId, fileName, contentType, memoryStream);
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

public record Attachment(string Id, string FileName, string ContentType, MemoryStream Stream);