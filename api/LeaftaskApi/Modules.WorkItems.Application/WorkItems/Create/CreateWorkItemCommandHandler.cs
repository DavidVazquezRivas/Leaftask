using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Application.WorkItems;
using Modules.WorkItems.Application.WorkItems.GetWorkItemDetails;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Entities.Properties;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.WorkItems.Create;

public sealed class CreateWorkItemCommandHandler(
    IWorkItemRepository workItemRepository,
    IProjectReadModelRepository projectReadModelRepository,
    IWorkItemConfigurationRepository configurationRepository,
    IUserReadModelRepository userReadModelRepository,
    IFieldRepository fieldRepository,
    IGetWorkItemDetailsQueryService detailsQueryService)
    : ICommandHandler<CreateWorkItemCommand, Result<WorkItemDetailDto>>
{
    public async Task<Result<WorkItemDetailDto>> Handle(
        CreateWorkItemCommand command,
        CancellationToken cancellationToken)
    {
        ProjectReadModel? project = await projectReadModelRepository.GetByIdAsync(
            command.ProjectId, cancellationToken);

        if (project is null)
        {
            return Result.Failure<WorkItemDetailDto>(WorkItemErrors.ProjectNotFound);
        }

        WorkItemStatus? status = await configurationRepository.GetStatusByIdAsync(
            command.StatusId, cancellationToken);

        if (status is null)
        {
            return Result.Failure<WorkItemDetailDto>(WorkItemErrors.StatusNotFound);
        }

        WorkItemType? type = await configurationRepository.GetTypeByIdAsync(
            command.TypeId, cancellationToken);

        if (type is null)
        {
            return Result.Failure<WorkItemDetailDto>(WorkItemErrors.TypeNotFound);
        }

        UserReadModel? assignee = null;
        if (command.AssigneeId.HasValue)
        {
            assignee = await userReadModelRepository.GetByIdAsync(command.AssigneeId.Value, cancellationToken);
            if (assignee is null)
            {
                return Result.Failure<WorkItemDetailDto>(WorkItemErrors.AssigneeNotFound);
            }
        }

        // parentId == projectId means "root level" — stored as null
        Guid? resolvedParentId = null;
        if (command.ParentId != command.ProjectId)
        {
            bool parentExists = await workItemRepository.ExistsInProjectAsync(
                command.ParentId, command.ProjectId, cancellationToken);

            if (!parentExists)
            {
                return Result.Failure<WorkItemDetailDto>(WorkItemErrors.ParentNotFound);
            }

            resolvedParentId = command.ParentId;
        }

        int code = await workItemRepository.GetNextCodeAsync(command.ProjectId, cancellationToken);

        WorkItem workItem = WorkItem.Create(
            code,
            command.Title,
            command.Description,
            command.Estimation,
            DateTime.UtcNow.AddDays(30),
            project,
            status,
            type,
            assignee,
            resolvedParentId);

        await workItemRepository.AddAsync(workItem, cancellationToken);
        await workItemRepository.SaveChangesAsync(cancellationToken);

        if (command.CustomFields.Count > 0)
        {
            Result<List<WorkItemChange>> customFieldResult = await WorkItemCustomFieldsService.ApplyAsync(
                workItem, command.CustomFields, fieldRepository, cancellationToken);

            if (customFieldResult.IsFailure)
            {
                return Result.Failure<WorkItemDetailDto>(customFieldResult.Error);
            }

            await fieldRepository.SaveChangesAsync(cancellationToken);
        }

        WorkItemDetailDto? detail = await detailsQueryService.GetWorkItemDetailsAsync(
            command.ProjectId, workItem.Id, cancellationToken);

        return Result.Success(detail!);
    }
}
