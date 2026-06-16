using BuildingBlocks.Application.Commands;
using Modules.Agents.Domain.Entities;
using Modules.Agents.Domain.Repositories;

namespace Modules.Agents.Application.Projects.Create;

public sealed class CreateProjectReadModelOnProjectCreatedCommandHandler(
    IProjectReadModelRepository projectReadModelRepository)
    : ICommandHandler<CreateProjectReadModelOnProjectCreatedCommand>
{
    public async Task Handle(
        CreateProjectReadModelOnProjectCreatedCommand request,
        CancellationToken cancellationToken)
    {
        ProjectReadModel? existing = await projectReadModelRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (existing is not null)
            return;

        ProjectReadModel model = new(request.ProjectId, request.Name);
        await projectReadModelRepository.AddAsync(model, cancellationToken);
        await projectReadModelRepository.SaveChangesAsync(cancellationToken);
    }
}
