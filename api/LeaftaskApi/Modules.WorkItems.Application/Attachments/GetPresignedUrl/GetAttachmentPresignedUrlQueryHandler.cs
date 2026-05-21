using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.Attachments.GetPresignedUrl;

public sealed class GetAttachmentPresignedUrlQueryHandler(
    IWorkItemRepository workItemRepository,
    IFileStorage fileStorage)
    : IQueryHandler<GetAttachmentPresignedUrlQuery, Result<PresignedUrlDto>>
{
    public async Task<Result<PresignedUrlDto>> Handle(
        GetAttachmentPresignedUrlQuery query,
        CancellationToken cancellationToken)
    {
        bool exists = await workItemRepository.GetByIdTrackedAsync(query.WorkItemId, query.ProjectId, cancellationToken) is not null;
        if (!exists)
            return Result.Failure<PresignedUrlDto>(WorkItemErrors.WorkItemNotFound);

        string objectKey = $"workitems/{query.WorkItemId}/{Guid.NewGuid()}/{query.FileName}";

        PresignedUploadResult result = await fileStorage.GetPresignedPutUrlAsync(objectKey, cancellationToken: cancellationToken);

        return Result.Success(new PresignedUrlDto(result.PresignedUrl, result.PublicUrl));
    }
}
