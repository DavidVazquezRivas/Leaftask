using System.Globalization;
using BuildingBlocks.Application.Queries;
using Microsoft.EntityFrameworkCore;
using Modules.Organizations.Application.Members.GetMembers;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.DrivenInfrastructure.Persistence;

namespace Modules.Organizations.DrivenInfrastructure.Queries;

public sealed class GetOrganizationMembersQueryService(OrganizationDbContext dbContext)
    : IGetOrganizationMembersQueryService
{
    private static readonly IReadOnlyDictionary<string, CursorSortFieldDefinition<MemberRow>> SortFields =
        new Dictionary<string, CursorSortFieldDefinition<MemberRow>>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = new("id", member => member.Id, value => Guid.Parse(value), value => ((Guid)value).ToString("D")),
            ["name"] = new("name", member => member.Name, value => value, value => (string)value),
            ["email"] = new("email", member => member.Email, value => value, value => (string)value),
            ["role"] = new("role", member => member.Role, value => Guid.Parse(value), value => ((Guid)value).ToString("D"))
        };

    private static readonly IReadOnlyList<string> DefaultSort = ["name:asc", "id:asc"];
    private const string IdSort = "id:asc";

    public async Task<PaginatedResult<OrganizationMemberDto>> GetOrganizationMembersAsync(
        Guid organizationId,
        int limit,
        string? cursor,
        IReadOnlyCollection<string> sort,
        CancellationToken cancellationToken = default)
    {
        List<MemberRow> members = await dbContext.OrganizationInvitations
            .AsNoTracking()
            .Where(invitation => invitation.OrganizationId == organizationId && invitation.Status == InvitationStatus.Accepted)
            .Join(
                dbContext.UserReadModels.AsNoTracking(),
                invitation => invitation.UserId,
                user => user.Id,
                (invitation, user) => new MemberRow(
                    user.Id,
                    $"{user.FirstName} {user.LastName}",
                    user.Email,
                    invitation.OrganizationRoleId))
            .ToListAsync(cancellationToken);

        IReadOnlyCollection<string> effectiveSort = NormalizeSort(sort);

        return CursorPaginationHelper.Paginate(
            members,
            limit,
            cursor,
            effectiveSort,
            SortFields,
            DefaultSort,
            member => new OrganizationMemberDto(member.Id, member.Name, member.Email, member.Role));
    }

    private static IReadOnlyCollection<string> NormalizeSort(IReadOnlyCollection<string> sort) =>
        sort.Count == 0 || sort.Any(item => item.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
            ? sort
            : [.. sort, IdSort];

    private sealed record MemberRow(Guid Id, string Name, string Email, Guid Role);
}
