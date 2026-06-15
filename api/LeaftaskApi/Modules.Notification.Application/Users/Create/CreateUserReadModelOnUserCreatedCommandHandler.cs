using BuildingBlocks.Application.Commands;
using Modules.Notification.Domain.Entities;
using Modules.Notification.Domain.Repositories;

namespace Modules.Notification.Application.Users.Create;

public sealed class CreateUserReadModelOnUserCreatedCommandHandler(IUserReadModelRepository userReadModelRepository)
    : ICommandHandler<CreateUserReadModelOnUserCreatedCommand>
{
    public async Task Handle(CreateUserReadModelOnUserCreatedCommand request, CancellationToken cancellationToken)
    {
        bool exists = await userReadModelRepository.ExistsByIdAsync(request.UserId, cancellationToken);
        if (exists) return;

        UserReadModel user = new(request.UserId, request.FirstName, request.LastName);
        await userReadModelRepository.AddAsync(user, cancellationToken);
        await userReadModelRepository.SaveChangesAsync(cancellationToken);
    }
}
