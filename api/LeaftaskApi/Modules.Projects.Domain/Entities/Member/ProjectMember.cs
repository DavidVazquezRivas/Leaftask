using BuildingBlocks.Domain.Entities;
using Modules.Projects.Domain.Entities.Role;

namespace Modules.Projects.Domain.Entities.Member;

public sealed class ProjectMember : Entity
{
    private ProjectMember() { }

    public ProjectMember(Guid id, Guid memberId, MemberType memberType, ProjectRole role, Project project)
    {
        Id = id;
        MemberId = memberId;
        MemberType = memberType;
        Role = role;
        Project = project;
        JoinedAt = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public Guid MemberId { get; }
    public MemberType MemberType { get; }
    public ProjectRole Role { get; private set; }
    public Project Project { get; }
    public DateTime JoinedAt { get; }

    public void UpdateRole(ProjectRole role)
    {
        Role = role;
    }
}
