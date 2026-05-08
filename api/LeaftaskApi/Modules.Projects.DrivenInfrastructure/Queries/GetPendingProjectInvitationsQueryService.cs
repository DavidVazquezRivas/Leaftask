using Microsoft.EntityFrameworkCore;
using Modules.Projects.Application.Invitations.GetPending;
using Modules.Projects.Domain.Entities.Invitation;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivenInfrastructure.Queries;

public sealed class GetPendingProjectInvitationsQueryService(ProjectsDbContext dbContext)
    : IGetPendingProjectInvitationsQueryService
{
    public async Task<IReadOnlyList<ProjectInvitationDto>> GetPendingAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        List<InvitationRow> rows = await (
            from i in dbContext.ProjectInvitations.AsNoTracking()
            join r in dbContext.ProjectRoles.AsNoTracking() on i.RoleId equals r.Id
            join u in dbContext.UserReadModels.AsNoTracking() on i.InviteeId equals u.Id into userGroup
            from u in userGroup.DefaultIfEmpty()
            where i.ProjectId == projectId && i.Status == InvitationStatus.Pending
            orderby i.InvitedAt descending
            select new InvitationRow(
                i.Id,
                i.InviteeId,
                i.InviteeType,
                u != null ? u.FirstName : null,
                u != null ? u.LastName : null,
                u != null ? u.Email : null,
                r.Id,
                r.Name))
            .ToListAsync(cancellationToken);

        return rows.Select(ToDto).ToList();
    }

    private static ProjectInvitationDto ToDto(InvitationRow row)
    {
        string name = row.InviteeType == MemberType.User
            ? $"{row.FirstName} {row.LastName}".Trim()
            : "Agent";

        string? email = row.InviteeType == MemberType.User ? row.Email : null;

        return new ProjectInvitationDto(
            row.Id,
            new ProjectInvitationUserDto(row.InviteeId, name, email),
            new ProjectInvitationRoleDto(row.RoleId, row.RoleName));
    }

    private sealed record InvitationRow(
        Guid Id,
        Guid InviteeId,
        MemberType InviteeType,
        string? FirstName,
        string? LastName,
        string? Email,
        Guid RoleId,
        string RoleName);
}
