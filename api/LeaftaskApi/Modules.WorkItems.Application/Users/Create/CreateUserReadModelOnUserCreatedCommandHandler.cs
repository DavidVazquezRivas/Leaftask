using BuildingBlocks.Application.Commands;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.Users.Create;

public sealed class CreateUserReadModelOnUserCreatedCommandHandler(
    IUserReadModelRepository userReadModelRepository,
    IWorkItemRepository workItemRepository)
    : ICommandHandler<CreateUserReadModelOnUserCreatedCommand>
{
    public async Task Handle(CreateUserReadModelOnUserCreatedCommand request, CancellationToken cancellationToken)
    {
        bool exists = await userReadModelRepository.ExistsByIdAsync(request.UserId, cancellationToken);
        if (exists) return;

        UserReadModel userReadModel = new(request.UserId, request.FirstName, request.LastName);
        await userReadModelRepository.AddAsync(userReadModel, cancellationToken);
        await workItemRepository.SaveChangesAsync(cancellationToken);
    }
}
