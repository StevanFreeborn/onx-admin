namespace OnxAdmin.API.Requests;

record DeleteAttachmentRequest(
  [FromRoute] string AttachmentId,
  [FromServices] IAttachmentService AttachmentService
);