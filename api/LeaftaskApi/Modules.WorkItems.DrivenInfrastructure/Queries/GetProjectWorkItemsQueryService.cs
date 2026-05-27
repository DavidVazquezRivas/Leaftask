using System.Globalization;
using BuildingBlocks.Application.Queries;
using Microsoft.EntityFrameworkCore;
using Modules.WorkItems.Application.WorkItems.GetProjectWorkItems;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivenInfrastructure.Queries;

public sealed class GetProjectWorkItemsQueryService(WorkItemsDbContext dbContext)
    : IGetProjectWorkItemsQueryService
{
    private const string IdSort = "id:asc";

    private static readonly IReadOnlyDictionary<string, CursorSortFieldDefinition<WorkItemRow>> SortFields =
        new Dictionary<string, CursorSortFieldDefinition<WorkItemRow>>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = new("id", row => row.Id, value => Guid.Parse(value), value => ((Guid)value).ToString("D")),
            ["code"] = new("code", row => row.Code, value => int.Parse(value, CultureInfo.InvariantCulture), value => ((int)value).ToString(CultureInfo.InvariantCulture)),
            ["title"] = new("title", row => row.Title, value => value, value => (string)value),
            ["progress"] = new("progress", row => row.Progress, value => int.Parse(value, CultureInfo.InvariantCulture), value => ((int)value).ToString(CultureInfo.InvariantCulture))
        };

    private static readonly IReadOnlyList<string> DefaultSort = ["code:asc", "id:asc"];

    public async Task<PaginatedResult<WorkItemListDto>> GetProjectWorkItemsAsync(
        Guid projectId,
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        CancellationToken cancellationToken = default)
    {
        List<WorkItemRow> workItems = await dbContext.WorkItems
            .AsNoTracking()
            .Where(wi => EF.Property<Guid>(wi, "project_read_model_id") == projectId)
            .Select(wi => new WorkItemRow(
                wi.Id,
                wi.Code,
                $"{wi.Project.Abbreviation}-{wi.Code}",
                wi.Title,
                wi.Estimation,
                wi.Progress,
                wi.Status.Id,
                wi.Type.Id,
                wi.Asignee != null ? wi.Asignee.Id : (Guid?)null,
                wi.Asignee != null ? wi.Asignee.FirstName : null,
                wi.Asignee != null ? wi.Asignee.LastName : null,
                wi.ParentId))
            .ToListAsync(cancellationToken);

        Dictionary<Guid, decimal> dedicationByItem = await dbContext.WorkLogs
            .AsNoTracking()
            .Where(wl => EF.Property<Guid>(wl.WorkItem, "project_read_model_id") == projectId)
            .GroupBy(wl => wl.WorkItem.Id)
            .Select(g => new { WorkItemId = g.Key, Total = g.Sum(wl => wl.Hours) })
            .ToDictionaryAsync(x => x.WorkItemId, x => x.Total, cancellationToken);

        IReadOnlyCollection<string> effectiveSort = NormalizeSort(sort);

        return CursorPaginationHelper.Paginate(
            workItems,
            limit,
            cursor,
            effectiveSort,
            SortFields,
            DefaultSort,
            row => new WorkItemListDto(
                row.Id,
                row.CodeFormatted,
                row.Title,
                row.Estimation,
                row.Progress / 100f,
                row.AssigneeId.HasValue
                    ? new WorkItemAssigneeDto(row.AssigneeId.Value, row.AssigneeFirstName!, row.AssigneeLastName!)
                    : null,
                dedicationByItem.TryGetValue(row.Id, out decimal hours) ? (float?)hours : null,
                row.TypeId,
                row.StatusId,
                row.ParentId));
    }

    private static IReadOnlyCollection<string> NormalizeSort(IReadOnlyCollection<string> sort) =>
        sort.Count == 0 || sort.Any(item => item.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
            ? sort
            : [.. sort, IdSort];

    private sealed record WorkItemRow(
        Guid Id,
        int Code,
        string CodeFormatted,
        string Title,
        decimal Estimation,
        int Progress,
        Guid StatusId,
        Guid TypeId,
        Guid? AssigneeId,
        string? AssigneeFirstName,
        string? AssigneeLastName,
        Guid? ParentId);
}
