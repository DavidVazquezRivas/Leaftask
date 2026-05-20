using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.WorkLogs.Delete;

public sealed class DeleteWorkLogCommandHandler(
    IWorkItemRepository workItemRepository,
    IWorkLogRepository workLogRepository,
    IUserContext userContext)
    : ICommandHandler<DeleteWorkLogCommand, Result>
{
    public async Task<Result> Handle(DeleteWorkLogCommand command, CancellationToken cancellationToken)
    {
        bool workItemExists = await workItemRepository.ExistsInProjectAsync(
            command.WorkItemId, command.ProjectId, cancellationToken);

        if (!workItemExists)
        {
            return Result.Failure(WorkItemErrors.WorkItemNotFound);
        }

        WorkLog? workLog = await workLogRepository.GetByIdTrackedAsync(
            command.LogId, command.WorkItemId, cancellationToken);

        if (workLog is null)
        {
            return Result.Failure(WorkItemErrors.WorkLogNotFound);
        }

        if (workLog.User.Id != userContext.UserId)
        {
            return Result.Failure(WorkItemErrors.WorkLogNotOwner);
        }

        workLogRepository.Remove(workLog);
        await workLogRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
