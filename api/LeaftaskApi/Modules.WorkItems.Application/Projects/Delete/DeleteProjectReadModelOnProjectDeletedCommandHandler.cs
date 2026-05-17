using BuildingBlocks.Application.Commands;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.Projects.Delete;

public sealed class DeleteProjectReadModelOnProjectDeletedCommandHandler(
    IProjectReadModelRepository projectReadModelRepository,
    IWorkItemRepository workItemRepository)
    : ICommandHandler<DeleteProjectReadModelOnProjectDeletedCommand>
{
    public async Task Handle(DeleteProjectReadModelOnProjectDeletedCommand request, CancellationToken cancellationToken)
    {
        ProjectReadModel? projectReadModel = await projectReadModelRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (projectReadModel is null) return;

        projectReadModelRepository.Remove(projectReadModel);
        await workItemRepository.SaveChangesAsync(cancellationToken);
    }
}
