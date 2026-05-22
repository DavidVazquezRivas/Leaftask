using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.WorkItems.Application.Attachments;
using Modules.WorkItems.Application.Attachments.Delete;
using Modules.WorkItems.Application.Attachments.GetPresignedUrl;
using Modules.WorkItems.Application.Attachments.Upload;

namespace Modules.WorkItems.DrivingInfrastructure.Controllers;

[Authorize]
[Route("api/v1/projects/{projectId:guid}/work-items/{itemId:guid}/attachments")]
public sealed class WorkItemAttachmentsController : ApiBaseController
{
    [HttpGet("presigned-upload")]
    public async Task<IActionResult> GetPresignedUploadUrl(
        Guid projectId,
        Guid itemId,
        [FromQuery] string fileName,
        CancellationToken cancellationToken = default)
    {
        Result<PresignedUrlDto> result = await Sender.Send(
            new GetAttachmentPresignedUrlQuery(projectId, itemId, fileName),
            cancellationToken);

        return HandleResult(result);
    }

    [HttpPost]
    [RequestSizeLimit(50 * 1024 * 1024)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadAttachment(
        Guid projectId,
        Guid itemId,
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        await using Stream stream = file.OpenReadStream();

        Result<AttachmentDto> result = await Sender.Send(
            new UploadAttachmentCommand(
                projectId, itemId, stream, file.FileName, file.ContentType, file.Length),
            cancellationToken);

        return HandleResult(result, 201);
    }

    [HttpDelete("{attachmentId:guid}")]
    public async Task<IActionResult> DeleteAttachment(
        Guid projectId,
        Guid itemId,
        Guid attachmentId,
        CancellationToken cancellationToken = default)
    {
        Result result = await Sender.Send(
            new DeleteAttachmentCommand(projectId, itemId, attachmentId),
            cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error);
        }

        return NoContent();
    }
}
