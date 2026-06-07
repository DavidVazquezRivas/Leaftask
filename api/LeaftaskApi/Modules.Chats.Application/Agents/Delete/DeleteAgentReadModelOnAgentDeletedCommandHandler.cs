using BuildingBlocks.Application.Commands;
using Modules.Chats.Domain.Repositories;

namespace Modules.Chats.Application.Agents.Delete;

public sealed class DeleteAgentReadModelOnAgentDeletedCommandHandler(
    IAgentReadModelRepository agentReadModelRepository)
    : ICommandHandler<DeleteAgentReadModelOnAgentDeletedCommand>
{
    public async Task Handle(DeleteAgentReadModelOnAgentDeletedCommand request, CancellationToken cancellationToken)
    {
        await agentReadModelRepository.RemoveByIdAsync(request.AgentId, cancellationToken);
    }
}
