using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.Domain.Entities.Role;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Members.UpdateRole;

public sealed class UpdateProjectMemberRoleCommandHandler(
    IProjectMemberRepository memberRepository,
    IProjectRoleRepository roleRepository)
    : ICommandHandler<UpdateProjectMemberRoleCommand, Result>
{
    public async Task<Result> Handle(UpdateProjectMemberRoleCommand command, CancellationToken cancellationToken)
    {
        ProjectMember? member = await memberRepository.GetByMemberIdTrackedAsync(
            command.ProjectId, command.MemberId, cancellationToken);

        if (member is null)
        {
            return Result.Failure(ProjectErrors.MemberNotFound);
        }

        ProjectRole? role = await roleRepository.GetByIdTrackedAsync(
            command.ProjectId, command.RoleId, cancellationToken);

        if (role is null)
        {
            return Result.Failure(ProjectErrors.RoleNotFound);
        }

        member.UpdateRole(role);
        await memberRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
