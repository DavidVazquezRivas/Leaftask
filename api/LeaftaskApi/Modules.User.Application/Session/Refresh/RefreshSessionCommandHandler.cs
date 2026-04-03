using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Users.Application.Session.Jwt;
using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Errors;
using Modules.Users.Domain.Factories;
using Modules.Users.Domain.Repositories;

namespace Modules.Users.Application.Session.Refresh;

internal class RefreshSessionCommandHandler(
    IUserRepository userRepository,
    IJwtService jwtService,
    IRefreshTokenFactory refreshTokenFactory)
    : ICommandHandler<RefreshSessionCommand, Result<SessionResponse>>
{
    public async Task<Result<SessionResponse>> Handle(RefreshSessionCommand request,
        CancellationToken cancellationToken)
    {
        User user = await userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (user is null)
        {
            return Result.Failure<SessionResponse>(UserErrors.InvalidRefreshToken);
        }

        Result<RefreshToken> rotationResult = user.RotateRefreshToken(request.RefreshToken, refreshTokenFactory);

        if (rotationResult.IsFailure)
        {
            return Result.Failure<SessionResponse>(rotationResult.Error);
        }

        RefreshToken newRefreshToken = rotationResult.Value;

        string newAccessToken = jwtService.GenerateToken(user);

        await userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(new SessionResponse(newAccessToken, newRefreshToken.Token,
            newRefreshToken.ExpiresAt));
    }
}
