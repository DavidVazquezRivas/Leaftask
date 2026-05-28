using BuildingBlocks.Application.Commands;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.Domain.Repositories;

namespace Modules.Chats.Application.Users.Create;

public sealed class CreateUserReadModelOnUserCreatedCommandHandler(
    IUserReadModelRepository userReadModelRepository)
    : ICommandHandler<CreateUserReadModelOnUserCreatedCommand>
{
    public async Task Handle(CreateUserReadModelOnUserCreatedCommand request, CancellationToken cancellationToken)
    {
        bool exists = await userReadModelRepository.ExistsByIdAsync(request.UserId, cancellationToken);
        if (exists) return;

        UserReadModel userReadModel = new(request.UserId, request.FirstName, request.LastName);
        await userReadModelRepository.AddAsync(userReadModel, cancellationToken);
    }
}
