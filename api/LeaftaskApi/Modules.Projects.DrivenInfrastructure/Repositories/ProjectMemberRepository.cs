using Microsoft.EntityFrameworkCore;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivenInfrastructure.Repositories;

public sealed class ProjectMemberRepository(ProjectsDbContext dbContext) : IProjectMemberRepository
{
    public async Task<ProjectMember?> GetByMemberIdTrackedAsync(Guid projectId, Guid memberId, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectMembers
            .Include(m => m.Role)
            .FirstOrDefaultAsync(
                m => EF.Property<Guid>(m, "project_id") == projectId && m.MemberId == memberId,
                cancellationToken);

    public async Task<bool> ExistsByMemberIdAsync(Guid projectId, Guid memberId, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectMembers
            .AsNoTracking()
            .AnyAsync(m => EF.Property<Guid>(m, "project_id") == projectId && m.MemberId == memberId,
                cancellationToken);

    public async Task<(int People, int Agents)> GetCountByProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var counts = await dbContext.ProjectMembers
            .AsNoTracking()
            .Where(m => EF.Property<Guid>(m, "project_id") == projectId)
            .GroupBy(m => m.MemberType)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        int people = counts.FirstOrDefault(c => c.Type == MemberType.User)?.Count ?? 0;
        int agents = counts.FirstOrDefault(c => c.Type == MemberType.Agent)?.Count ?? 0;

        return (people, agents);
    }

    public async Task AddAsync(ProjectMember member, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectMembers.AddAsync(member, cancellationToken);

    public void Remove(ProjectMember member) =>
        dbContext.ProjectMembers.Remove(member);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
