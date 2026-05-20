using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.WorkLogs.Create;

public sealed class LogWorkCommandHandler(
    IWorkItemRepository workItemRepository,
    IUserReadModelRepository userReadModelRepository,
    IWorkLogRepository workLogRepository,
    IUserContext userContext)
    : ICommandHandler<LogWorkCommand, Result<WorkLogDto>>
{
    public async Task<Result<WorkLogDto>> Handle(LogWorkCommand command, CancellationToken cancellationToken)
    {
        WorkItem? workItem = await workItemRepository.GetByIdTrackedAsync(
            command.WorkItemId, command.ProjectId, cancellationToken);

        if (workItem is null)
        {
            return Result.Failure<WorkLogDto>(WorkItemErrors.WorkItemNotFound);
        }

        UserReadModel? user = await userReadModelRepository.GetByIdAsync(userContext.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<WorkLogDto>(WorkItemErrors.AssigneeNotFound);
        }

        DateTime date = command.Date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        WorkLog workLog = new(Guid.NewGuid(), date, command.Dedication, command.Description, workItem, user);
        await workLogRepository.AddAsync(workLog, cancellationToken);
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
