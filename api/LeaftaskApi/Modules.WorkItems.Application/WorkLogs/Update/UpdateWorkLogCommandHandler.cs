using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.WorkLogs.Update;

public sealed class UpdateWorkLogCommandHandler(
    IWorkItemRepository workItemRepository,
    IWorkLogRepository workLogRepository,
    IUserContext userContext)
    : ICommandHandler<UpdateWorkLogCommand, Result<WorkLogDto>>
{
    public async Task<Result<WorkLogDto>> Handle(UpdateWorkLogCommand command, CancellationToken cancellationToken)
    {
        bool workItemExists = await workItemRepository.ExistsInProjectAsync(
            command.WorkItemId, command.ProjectId, cancellationToken);

        if (!workItemExists)
        {
            return Result.Failure<WorkLogDto>(WorkItemErrors.WorkItemNotFound);
        }

        WorkLog? workLog = await workLogRepository.GetByIdTrackedAsync(
            command.LogId, command.WorkItemId, cancellationToken);

        if (workLog is null)
        {
            return Result.Failure<WorkLogDto>(WorkItemErrors.WorkLogNotFound);
        }

        if (workLog.User.Id != userContext.UserId)
        {
            return Result.Failure<WorkLogDto>(WorkItemErrors.WorkLogNotOwner);
        }

        DateTime? date = command.Date.HasValue
            ? command.Date.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)
            : null;

        workLog.Update(date, command.Dedication, command.Description);
        await workLogRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(ToDto(workLog));
    }

    private static WorkLogDto ToDto(WorkLog log) => new(
        log.Id,
        (float)log.Hours,
        DateOnly.FromDateTime(log.Date),
        new WorkLogUserDto(log.User.Id, log.User.FirstName, log.User.LastName),
        log.Comment);
}
