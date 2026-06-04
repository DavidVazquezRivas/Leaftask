using System.ComponentModel;
using BuildingBlocks.Domain.Result;
using BuildingBlocks.DrivingInfrastructure.Tools;
using MediatR;
using Microsoft.SemanticKernel;
using Modules.WorkItems.Application.Attachments;
using Modules.WorkItems.Application.Attachments.Delete;
using Modules.WorkItems.Application.Attachments.GetPresignedUrl;

namespace Modules.WorkItems.DrivingInfrastructure.Tools;

public class WorkItemAttachmentsAiTool(ISender sender, IAiResponseFormatter formatter) : IAiTool
{
    [KernelFunction("GetWorkItemAttachmentUploadUrl")]
    [Description(
        "Generates a presigned URL that allows uploading a file attachment directly to a work item. Share this URL with the user so they can upload the file from their client.")]
    public async Task<string> GetPresignedUploadUrlAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description("The unique identifier (GUID) of the work item.")]
        Guid workItemId,
        [Description("The name of the file to upload (e.g., 'report.pdf').")]
        string fileName,
        CancellationToken cancellationToken = default)
    {
        Result<PresignedUrlDto> result = await sender.Send(
            new GetAttachmentPresignedUrlQuery(projectId, workItemId, fileName), cancellationToken);

        if (result.IsFailure)
            return formatter.FormatFailure(nameof(GetPresignedUploadUrlAsync), result.Error.Description);

        return formatter.FormatResponse(result.Value);
    }

    [KernelFunction("DeleteWorkItemAttachment")]
    [Description(
        "Permanently deletes a file attachment from a work item. Use carefully as this cannot be undone. Find the attachment ID first using 'GetWorkItemDetails'.")]
    public async Task<string> DeleteAttachmentAsync(
        [Description("The unique identifier (GUID) of the project.")]
        Guid projectId,
        [Description("The unique identifier (GUID) of the work item.")]
        Guid workItemId,
        [Description(
            "The unique identifier (GUID) of the attachment to delete. Find this ID first using 'GetWorkItemDetails'.")]
        Guid attachmentId,
        CancellationToken cancellationToken = default)
    {
        Result result = await sender.Send(
            new DeleteAttachmentCommand(projectId, workItemId, attachmentId), cancellationToken);

        if (result.IsFailure)
            return formatter.FormatFailure(nameof(DeleteAttachmentAsync), result.Error.Description);

        return formatter.FormatMessage("Attachment deleted successfully.");
    }
}
