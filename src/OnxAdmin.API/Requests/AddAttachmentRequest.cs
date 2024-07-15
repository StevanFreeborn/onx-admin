namespace OnxAdmin.API.Requests;

record AddAttachmentRequest(
  [FromForm] IFormFile Attachment,
  [FromServices] IAttachmentService AttachmentService
);