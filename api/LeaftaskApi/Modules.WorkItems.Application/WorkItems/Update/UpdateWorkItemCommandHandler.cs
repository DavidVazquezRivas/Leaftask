using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Application.WorkItems;
using Modules.WorkItems.Application.WorkItems.GetWorkItemDetails;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Entities.Properties;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.WorkItems.Update;

public sealed class UpdateWorkItemCommandHandler(
    IWorkItemRepository workItemRepository,
    IWorkItemConfigurationRepository configurationRepository,
    IUserReadModelRepository userReadModelRepository,
    IFieldRepository fieldRepository,
    IGetWorkItemDetailsQueryService detailsQueryService,
    IUserContext userContext)
    : ICommandHandler<UpdateWorkItemCommand, Result<WorkItemDetailDto>>
{
    public async Task<Result<WorkItemDetailDto>> Handle(
        UpdateWorkItemCommand command,
        CancellationToken cancellationToken)
    {
        WorkItem? workItem = await workItemRepository.GetByIdTrackedAsync(
            command.WorkItemId, command.ProjectId, cancellationToken);

        if (workItem is null)
        {
            return Result.Failure<WorkItemDetailDto>(WorkItemErrors.WorkItemNotFound);
        }

        WorkItemStatus? newStatus = null;
        if (command.StatusId.HasValue)
        {
            newStatus = await configurationRepository.GetStatusByIdAsync(command.StatusId.Value, cancellationToken);
            if (newStatus is null)
            {
                return Result.Failure<WorkItemDetailDto>(WorkItemErrors.StatusNotFound);
            }
        }

        WorkItemType? newType = null;
        if (command.TypeId.HasValue)
        {
            newType = await configurationRepository.GetTypeByIdAsync(command.TypeId.Value, cancellationToken);
            if (newType is null)
            {
                return Result.Failure<WorkItemDetailDto>(WorkItemErrors.TypeNotFound);
            }
        }

        UserReadModel? newAssignee = null;
        if (command.UpdateAssignee && command.AssigneeId.HasValue)
        {
            newAssignee = await userReadModelRepository.GetByIdAsync(command.AssigneeId.Value, cancellationToken);
            if (newAssignee is null)
            {
                return Result.Failure<WorkItemDetailDto>(WorkItemErrors.AssigneeNotFound);
            }
        }

        if (command.UpdateParent && command.ParentId.HasValue)
        {
            bool parentExists = await workItemRepository.ExistsInProjectAsync(
                command.ParentId.Value, command.ProjectId, cancellationToken);
            if (!parentExists)
            {
                return Result.Failure<WorkItemDetailDto>(WorkItemErrors.ParentNotFound);
            }

            // Walk the ancestor chain to detect circular dependency
            Guid? ancestorId = command.ParentId;
            while (ancestorId.HasValue)
            {
                if (ancestorId.Value == command.WorkItemId)
                {
                    return Result.Failure<WorkItemDetailDto>(WorkItemErrors.CircularDependency);
                }
                WorkItem? ancestor = await workItemRepository.GetByIdAsync(ancestorId.Value, cancellationToken);
                ancestorId = ancestor?.ParentId;
            }
        }

        IReadOnlyList<WorkItemChange> changes = workItem.ApplyUpdate(
            command.Title,
            command.Description,
            command.Progress,
            command.Estimation,
            command.LimitDate,
            newStatus,
            newType,
            newAssignee,
            command.UpdateAssignee,
            command.ParentId,
            command.UpdateParent);

        Result<List<WorkItemChange>> customFieldResult = await WorkItemCustomFieldsService.ApplyAsync(
            workItem, command.CustomFields, fieldRepository, cancellationToken);

        if (customFieldResult.IsFailure)
        {
            return Result.Failure<WorkItemDetailDto>(customFieldResult.Error);
        }

        List<WorkItemChange> customFieldChanges = customFieldResult.Value;

        changes = [.. changes, .. customFieldChanges];

        if (changes.Count > 0)
        {
            UserReadModel? actor = await userReadModelRepository.GetByIdAsync(
                userContext.UserId, cancellationToken);

            if (actor is not null)
            {
                foreach (WorkItemChange change in changes)
                {
                    ActivityLog log = new(
                        Guid.NewGuid(),
                        change.FieldName,
                        change.OldValue,
                        change.NewValue,
                        DateTime.UtcNow,
                        workItem,
                        actor);

                    await workItemRepository.AddActivityLogAsync(log, cancellationToken);
                }
            }
        }

        await workItemRepository.SaveChangesAsync(cancellationToken);

        WorkItemDetailDto? detail = await detailsQueryService.GetWorkItemDetailsAsync(
            command.ProjectId, command.WorkItemId, cancellationToken);

        return Result.Success(detail!);
    }

}
