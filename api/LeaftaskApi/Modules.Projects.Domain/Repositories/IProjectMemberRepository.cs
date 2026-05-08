using Modules.Projects.Domain.Entities.Member;

namespace Modules.Projects.Domain.Repositories;

public interface IProjectMemberRepository
{
    Task<ProjectMember?> GetByMemberIdTrackedAsync(Guid projectId, Guid memberId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByMemberIdAsync(Guid projectId, Guid memberId, CancellationToken cancellationToken = default);
    Task<(int People, int Agents)> GetCountByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task AddAsync(ProjectMember member, CancellationToken cancellationToken = default);
    void Remove(ProjectMember member);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
