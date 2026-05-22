using System.Globalization;
using BuildingBlocks.Application.Queries;
using Microsoft.EntityFrameworkCore;
using Modules.Projects.Application.Management.GetMyProjects;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivenInfrastructure.Queries;

public sealed class GetMyProjectsQueryService(ProjectsDbContext dbContext) : IGetMyProjectsQueryService
{
    private const string IdSort = "id:asc";

    private static readonly IReadOnlyDictionary<string, CursorSortFieldDefinition<ProjectRow>> SortFields =
        new Dictionary<string, CursorSortFieldDefinition<ProjectRow>>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = new("id", row => row.Id, value => Guid.Parse(value), value => ((Guid)value).ToString("D")),
            ["name"] = new("name", row => row.Name, value => value, value => (string)value),
            ["abbreviation"] = new("abbreviation", row => row.Abbreviation, value => value, value => (string)value),
            ["createdAt"] = new("createdAt", row => row.CreatedAt,
                value => DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                value => ((DateTime)value).ToString("O", CultureInfo.InvariantCulture))
        };

    private static readonly IReadOnlyList<string> DefaultSort = ["createdAt:asc", "id:asc"];

    public async Task<PaginatedResult<SimpleProjectDto>> GetMyProjectsAsync(
        Guid userId,
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        CancellationToken cancellationToken = default)
    {
        List<ProjectRow> projects = await dbContext.Projects
            .AsNoTracking()
            .Where(project => project.OwnerType == OwnerType.User && project.OwnerId == userId)
            .Select(project => new ProjectRow(project.Id, project.Name, project.Abbreviation, project.CreatedAt))
            .ToListAsync(cancellationToken);

        IReadOnlyCollection<string> effectiveSort = NormalizeSort(sort);

        return CursorPaginationHelper.Paginate(
            projects,
            limit,
            cursor,
            effectiveSort,
            SortFields,
            DefaultSort,
            project => new SimpleProjectDto(project.Id, project.Name, project.Abbreviation));
    }

    private static IReadOnlyCollection<string> NormalizeSort(IReadOnlyCollection<string> sort) =>
        sort.Count == 0 || sort.Any(item => item.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
            ? sort
            : [.. sort, IdSort];

    private sealed record ProjectRow(Guid Id, string Name, string Abbreviation, DateTime CreatedAt);
}
