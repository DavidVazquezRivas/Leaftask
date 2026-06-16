using BuildingBlocks.Application.Commands;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.Agents.Delete;

public sealed class DeleteUserReadModelOnAgentDeletedCommandHandler(
    IUserReadModelRepository userReadModelRepository,
    IWorkItemRepository workItemRepository)
    : ICommandHandler<DeleteUserReadModelOnAgentDeletedCommand>
{
    public async Task Handle(DeleteUserReadModelOnAgentDeletedCommand request, CancellationToken cancellationToken)
    {
        UserReadModel? entry = await userReadModelRepository.GetByIdAsync(request.AgentId, cancellationToken);
        if (entry is null) return;

        userReadModelRepository.Remove(entry);
        await workItemRepository.SaveChangesAsync(cancellationToken);
    }
}
