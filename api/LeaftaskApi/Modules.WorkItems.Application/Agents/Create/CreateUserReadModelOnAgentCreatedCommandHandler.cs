using BuildingBlocks.Application.Commands;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.Agents.Create;

public sealed class CreateUserReadModelOnAgentCreatedCommandHandler(
    IUserReadModelRepository userReadModelRepository,
    IWorkItemRepository workItemRepository)
    : ICommandHandler<CreateUserReadModelOnAgentCreatedCommand>
{
    public async Task Handle(CreateUserReadModelOnAgentCreatedCommand request, CancellationToken cancellationToken)
    {
        bool exists = await userReadModelRepository.ExistsByIdAsync(request.AgentId, cancellationToken);
        if (exists) return;

        UserReadModel agentAsUser = new(request.AgentId, request.Name, string.Empty);
        await userReadModelRepository.AddAsync(agentAsUser, cancellationToken);
        await workItemRepository.SaveChangesAsync(cancellationToken);
    }
}
