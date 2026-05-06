using BuildingBlocks.Application.Commands;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Users.Create;

public sealed class CreateUserReadModelOnUserCreatedCommandHandler(
    IUserReadModelRepository userReadModelRepository)
    : ICommandHandler<CreateUserReadModelOnUserCreatedCommand>
{
    public async Task Handle(CreateUserReadModelOnUserCreatedCommand request, CancellationToken cancellationToken)
    {
        bool exists = await userReadModelRepository.ExistsByIdAsync(request.UserId, cancellationToken);
        if (exists)
        {
            return;
        }

        UserReadModel userReadModel = new(request.UserId, request.FirstName, request.LastName, request.Email);
        await userReadModelRepository.AddAsync(userReadModel, cancellationToken);
    }
}
