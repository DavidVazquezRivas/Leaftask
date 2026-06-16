using BuildingBlocks.Application.Commands;
using Modules.Projects.Domain.Entities.Member;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Agents.Delete;

public sealed class DeleteAgentReadModelOnAgentDeletedCommandHandler(
    IAgentReadModelRepository agentReadModelRepository,
    IProjectMemberRepository memberRepository)
    : ICommandHandler<DeleteAgentReadModelOnAgentDeletedCommand>
{
    public async Task Handle(DeleteAgentReadModelOnAgentDeletedCommand request, CancellationToken cancellationToken)
    {
        ProjectMember? member = await memberRepository.GetByMemberIdTrackedAsync(
            request.ProjectId, request.AgentId, cancellationToken);

        if (member is not null)
            memberRepository.Remove(member);

        await agentReadModelRepository.RemoveByIdAsync(request.AgentId, cancellationToken);
        await memberRepository.SaveChangesAsync(cancellationToken);
    }
}
