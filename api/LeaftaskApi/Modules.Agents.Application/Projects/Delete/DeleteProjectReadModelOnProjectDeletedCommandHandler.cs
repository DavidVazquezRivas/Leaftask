using BuildingBlocks.Application.Commands;
using Modules.Agents.Domain.Entities;
using Modules.Agents.Domain.Repositories;

namespace Modules.Agents.Application.Projects.Delete;

public sealed class DeleteProjectReadModelOnProjectDeletedCommandHandler(
    IProjectReadModelRepository projectReadModelRepository)
    : ICommandHandler<DeleteProjectReadModelOnProjectDeletedCommand>
{
    public async Task Handle(
        DeleteProjectReadModelOnProjectDeletedCommand request,
        CancellationToken cancellationToken)
    {
        ProjectReadModel? model = await projectReadModelRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (model is null)
            return;

        projectReadModelRepository.Remove(model);
        await projectReadModelRepository.SaveChangesAsync(cancellationToken);
    }
}
