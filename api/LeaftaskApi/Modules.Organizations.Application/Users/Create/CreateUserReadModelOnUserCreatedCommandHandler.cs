using BuildingBlocks.Application.Commands;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Repositories;

namespace Modules.Organizations.Application.Users.Create;

public sealed class CreateUserReadModelOnUserCreatedCommandHandler(IUserReadModelRepository userReadModelRepository)
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
