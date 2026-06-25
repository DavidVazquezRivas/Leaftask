using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Users.Application.Session.Jwt;
using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Errors;
using Modules.Users.Domain.Factories;
using Modules.Users.Domain.Repositories;

namespace Modules.Users.Application.Session.DevLogin;

public sealed class DevLoginCommandHandler(
    IUserRepository userRepository,
    IJwtService jwtService,
    IRefreshTokenFactory refreshTokenFactory)
    : ICommandHandler<DevLoginCommand, Result<SessionResponse>>
{
    public async Task<Result<SessionResponse>> Handle(DevLoginCommand command, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (user is null)
        {
            return Result.Failure<SessionResponse>(UserErrors.UserNotFound);
        }

        string accessToken = jwtService.GenerateToken(user);
        RefreshToken refreshToken = user.AddRefreshToken(refreshTokenFactory);
        await userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(new SessionResponse(accessToken, refreshToken.Token, refreshToken.ExpiresAt));
    }
}
