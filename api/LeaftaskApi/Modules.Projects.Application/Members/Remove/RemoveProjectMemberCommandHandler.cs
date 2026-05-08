using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Members.Remove;

public sealed class RemoveProjectMemberCommandHandler(IProjectMemberRepository memberRepository)
    : ICommandHandler<RemoveProjectMemberCommand, Result>
{
    public async Task<Result> Handle(RemoveProjectMemberCommand command, CancellationToken cancellationToken)
    {
        ProjectMember? member = await memberRepository.GetByMemberIdTrackedAsync(
            command.ProjectId, command.MemberId, cancellationToken);

        if (member is null)
        {
            return Result.Failure(ProjectErrors.MemberNotFound);
        }

        memberRepository.Remove(member);
        await memberRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
