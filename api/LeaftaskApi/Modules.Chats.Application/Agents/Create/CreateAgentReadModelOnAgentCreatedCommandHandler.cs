using BuildingBlocks.Application.Commands;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.Domain.Repositories;

namespace Modules.Chats.Application.Agents.Create;

public sealed class CreateAgentReadModelOnAgentCreatedCommandHandler(
    IAgentReadModelRepository agentReadModelRepository)
    : ICommandHandler<CreateAgentReadModelOnAgentCreatedCommand>
{
    public async Task Handle(CreateAgentReadModelOnAgentCreatedCommand request, CancellationToken cancellationToken)
    {
        bool exists = await agentReadModelRepository.ExistsByIdAsync(request.AgentId, cancellationToken);
        if (exists)
            return;

        AgentReadModel model = new(request.AgentId, request.Name);
        await agentReadModelRepository.AddAsync(model, cancellationToken);
        await agentReadModelRepository.SaveChangesAsync(cancellationToken);
    }
}
