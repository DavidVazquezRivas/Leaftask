using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Errors;
using Modules.Users.Domain.Repositories;

namespace Modules.Users.Application.Session.GetActive;

public sealed class GetActiveUserQueryHandler(
    IUserRepository userRepository,
    IUserContext userContext)
    : IQueryHandler<GetActiveUserQuery, Result<ActiveUserResponse>>
{
    public async Task<Result<ActiveUserResponse>> Handle(GetActiveUserQuery request, CancellationToken cancellationToken)
    {
        Guid userId = userContext.UserId;
        if (userId == Guid.Empty)
        {
            return Result.Failure<ActiveUserResponse>(UserErrors.Unauthenticated);
        }

        User? user = await userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<ActiveUserResponse>(UserErrors.UserNotFound);
        }

        ActiveUserResponse response = new(user.Id, user.FirstName, user.LastName, user.Email);

        return Result.Success(response);
    }
}
