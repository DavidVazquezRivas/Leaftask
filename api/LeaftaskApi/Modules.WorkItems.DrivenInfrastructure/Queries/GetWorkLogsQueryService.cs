using System.Globalization;
using BuildingBlocks.Application.Queries;
using Microsoft.EntityFrameworkCore;
using Modules.WorkItems.Application.WorkLogs;
using Modules.WorkItems.Application.WorkLogs.List;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivenInfrastructure.Queries;

public sealed class GetWorkLogsQueryService(WorkItemsDbContext dbContext) : IGetWorkLogsQueryService
{
    private const string IdSort = "id:asc";

    private static readonly IReadOnlyDictionary<string, CursorSortFieldDefinition<WorkLogRow>> SortFields =
        new Dictionary<string, CursorSortFieldDefinition<WorkLogRow>>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = new("id", row => row.Id, value => Guid.Parse(value), value => ((Guid)value).ToString("D")),
            ["date"] = new("date", row => row.Date, value => DateTime.Parse(value, CultureInfo.InvariantCulture), value => ((DateTime)value).ToString("O", CultureInfo.InvariantCulture)),
            ["dedication"] = new("dedication", row => row.Hours, value => decimal.Parse(value, CultureInfo.InvariantCulture), value => ((decimal)value).ToString(CultureInfo.InvariantCulture))
        };

    private static readonly IReadOnlyList<string> DefaultSort = ["date:desc", "id:asc"];

    public async Task<PaginatedResult<WorkLogDto>> GetWorkLogsAsync(
        Guid projectId,
        Guid workItemId,
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        CancellationToken cancellationToken = default)
    {
        List<WorkLogRow> rows = await dbContext.WorkLogs
            .AsNoTracking()
            .Where(wl => EF.Property<Guid>(wl, "work_item_id") == workItemId)
            .Select(wl => new WorkLogRow(
                wl.Id,
                wl.Date,
                wl.Hours,
                wl.Comment,
                wl.User.Id,
                wl.User.FirstName,
                wl.User.LastName))
            .ToListAsync(cancellationToken);

        IReadOnlyCollection<string> effectiveSort = NormalizeSort(sort);

        return CursorPaginationHelper.Paginate(
            rows,
            limit,
            cursor,
            effectiveSort,
            SortFields,
            DefaultSort,
            row => new WorkLogDto(
                row.Id,
                (float)row.Hours,
                DateOnly.FromDateTime(row.Date),
                new WorkLogUserDto(row.UserId, row.UserFirstName, row.UserLastName),
                row.Comment));
    }

    private static IReadOnlyCollection<string> NormalizeSort(IReadOnlyCollection<string> sort)
    {
        IReadOnlyCollection<string> baseSort = sort.Count == 0 ? DefaultSort : sort;
        return baseSort.Any(s => s.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
            ? baseSort
            : [.. baseSort, IdSort];
    }

    private sealed record WorkLogRow(
        Guid Id,
        DateTime Date,
        decimal Hours,
        string Comment,
        Guid UserId,
        string UserFirstName,
        string UserLastName);
}
