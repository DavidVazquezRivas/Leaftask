using System.Globalization;
using System.Text.Json;
using BuildingBlocks.Application.Queries;
using Microsoft.EntityFrameworkCore;
using Modules.Organizations.Application.Management.GetMyOrganizations;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.DrivenInfrastructure.Persistence;

namespace Modules.Organizations.DrivenInfrastructure.Queries;

public sealed class GetMyOrganizationsQueryService(OrganizationDbContext dbContext)
    : IGetMyOrganizationsQueryService
{
    private static readonly IReadOnlyDictionary<string, CursorSortFieldDefinition<OrganizationRow>> SortFields =
        new Dictionary<string, CursorSortFieldDefinition<OrganizationRow>>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = new("id", organization => organization.Id, value => Guid.Parse(value), value => ((Guid)value).ToString("D")),
            ["name"] = new("name", organization => organization.Name, value => value, value => (string)value),
            ["description"] = new("description", organization => organization.Description, value => value, value => (string)value),
            ["website"] = new("website", organization => organization.Website, value => value, value => (string)value),
            ["createdAt"] = new("createdAt", organization => organization.CreatedAt, value => DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind), value => ((DateTime)value).ToString("O", CultureInfo.InvariantCulture))
        };

    private static readonly IReadOnlyList<string> DefaultSort = ["createdAt:asc", "id:asc"];
    private const string IdSort = "id:asc";

    public async Task<PaginatedResult<SimpleOrganizationDto>> GetMyOrganizationsAsync(
        Guid userId,
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        CancellationToken cancellationToken = default)
    {
        List<OrganizationRow> organizations = await dbContext.Organizations
            .AsNoTracking()
            .Where(organization => organization.Invitations.Any(invitation =>
                invitation.UserId == userId && invitation.Status == InvitationStatus.Accepted))
            .Select(organization => new OrganizationRow(
                organization.Id,
                organization.Name,
                organization.Description,
                organization.Website,
                organization.CreatedAt))
            .ToListAsync(cancellationToken);

        IReadOnlyCollection<string> effectiveSort = NormalizeSort(sort);

        return CursorPaginationHelper.Paginate(
            organizations,
            limit,
            cursor,
            effectiveSort,
            SortFields,
            DefaultSort,
            organization => new SimpleOrganizationDto(organization.Id, organization.Name));
    }

    private static IReadOnlyCollection<string> NormalizeSort(IReadOnlyCollection<string> sort) =>
        sort.Count == 0 || sort.Any(item => item.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
            ? sort
            : [.. sort, IdSort];

    private sealed record OrganizationRow(Guid Id, string Name, string Description, string Website, DateTime CreatedAt);
}
