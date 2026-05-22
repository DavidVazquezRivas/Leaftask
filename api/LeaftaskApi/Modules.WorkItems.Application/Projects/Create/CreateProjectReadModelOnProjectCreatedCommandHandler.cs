using BuildingBlocks.Application.Commands;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.Projects.Create;

public sealed class CreateProjectReadModelOnProjectCreatedCommandHandler(
    IProjectReadModelRepository projectReadModelRepository,
    IWorkItemRepository workItemRepository)
    : ICommandHandler<CreateProjectReadModelOnProjectCreatedCommand>
{
    public async Task Handle(CreateProjectReadModelOnProjectCreatedCommand request, CancellationToken cancellationToken)
    {
        bool exists = await projectReadModelRepository.ExistsByIdAsync(request.ProjectId, cancellationToken);
        if (exists) return;

        ProjectReadModel projectReadModel = new(request.ProjectId, request.Abbreviation);
        await projectReadModelRepository.AddAsync(projectReadModel, cancellationToken);
        await workItemRepository.SaveChangesAsync(cancellationToken);
    }
}
