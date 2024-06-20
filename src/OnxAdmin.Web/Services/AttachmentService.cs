using System.IO.Abstractions;

using Microsoft.AspNetCore.Components.Forms;
using Microsoft.VisualBasic;

namespace OnxAdmin.Web.Services;

interface IAttachmentService
{
  Task<Guid> UploadAttachmentAsync(IBrowserFile file);
  string GetAttachmentExtensionAsync(string attachmentId);
  Task<string[]> GetAttachmentLinesAsync(string attachmentId);
}

class AttachmentService(IWebHostEnvironment env, IFileSystem fileSystem) : IAttachmentService
{
  private const long MaxFileSize = 1024 * 1024 * 1024; // 1 GB
  private const string AttachmentsDirectory = "Attachments";
  private readonly IWebHostEnvironment _env = env;
  private readonly IFileSystem _fileSystem = fileSystem;

  public string GetAttachmentExtensionAsync(string attachmentId)
  {
    var directoryPath = _fileSystem.Path.Combine(_env.ContentRootPath, AttachmentsDirectory, attachmentId);
    var filePath = _fileSystem.Directory.GetFiles(directoryPath).First();
    return _fileSystem.Path.GetExtension(filePath);
  }

  public async Task<string[]> GetAttachmentLinesAsync(string attachmentId)
  {
    var directoryPath = _fileSystem.Path.Combine(_env.ContentRootPath, AttachmentsDirectory, attachmentId);
    var filePath = _fileSystem.Directory.GetFiles(directoryPath).First();
    return await _fileSystem.File.ReadAllLinesAsync(filePath);
  }

  public async Task<Guid> UploadAttachmentAsync(IBrowserFile file)
  {
    var attachmentId = Guid.NewGuid();
    var trustedFileName = _fileSystem.Path.GetRandomFileName();
    var fileExtension = _fileSystem.Path.GetExtension(file.Name);
    var fileName = $"{trustedFileName}{fileExtension}";
    var directoryPath = _fileSystem.Path.Combine(_env.ContentRootPath, AttachmentsDirectory, attachmentId.ToString());
    var filePath = _fileSystem.Path.Combine(directoryPath, fileName);

    _fileSystem.Directory.CreateDirectory(directoryPath);
    await using var fileStream = _fileSystem.File.Create(filePath);
    await file.OpenReadStream(MaxFileSize).CopyToAsync(fileStream);
    return attachmentId;
  }
}