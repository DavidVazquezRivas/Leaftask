using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Errors;
using Modules.Users.Domain.Repositories;
using Modules.Users.Domain.Specifications;

namespace Modules.Users.Application.Session.Logout;

public sealed class LogoutCommandHandler(
    IUserRepository userRepository,
    IUserContext userContext)
    : ICommandHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;
        if (userId == Guid.Empty)
        {
            return Result.Failure(UserErrors.Unauthenticated);
        }

        UserWithActiveRefreshTokensSpecification specification = new(userId);
        User user = await userRepository.GetBySpecAsync(specification, cancellationToken);
        if (user is null)
        {
            return Result.Failure(UserErrors.UserNotFound);
        }

        user.RevokeAllRefreshTokens();
        await userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
