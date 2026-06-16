using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.WorkItems.Delete;

public sealed class DeleteWorkItemCommandHandler(IWorkItemRepository workItemRepository)
    : ICommandHandler<DeleteWorkItemCommand, Result>
{
    public async Task<Result> Handle(DeleteWorkItemCommand command, CancellationToken cancellationToken)
    {
        WorkItem? workItem = await workItemRepository.GetByIdTrackedAsync(
            command.WorkItemId, command.ProjectId, cancellationToken);

        if (workItem is null)
        {
            return Result.Failure(WorkItemErrors.WorkItemNotFound);
        }

        workItem.Delete();
        workItemRepository.Remove(workItem);
        await workItemRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
