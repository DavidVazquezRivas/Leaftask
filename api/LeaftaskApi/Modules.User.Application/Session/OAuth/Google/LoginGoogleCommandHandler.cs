using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Users.Application.Session.Jwt;
using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Factories;
using Modules.Users.Domain.Repositories;

namespace Modules.Users.Application.Session.OAuth.Google;

public sealed class LoginGoogleCommandHandler(
    IUserRepository userRepository,
    IGoogleTokenValidator googleTokenValidator,
    IJwtService jwtService,
    IRefreshTokenFactory refreshTokenFactory)
    : ICommandHandler<LoginGoogleCommand, Result<SessionResponse>>
{
    public async Task<Result<SessionResponse>> Handle(LoginGoogleCommand request, CancellationToken cancellationToken)
    {
        Result<GoogleUserPayload> validationResult =
            await googleTokenValidator.ValidateAsync(request.GoogleIdToken, cancellationToken);
        if (validationResult.IsFailure)
        {
            return Result<SessionResponse>.ValidationFailure(validationResult.Error);
        }

        GoogleUserPayload googleUser = validationResult.Value;
        User? user = await userRepository.GetByEmailAsync(googleUser.Email, cancellationToken);

        bool isNewUser = false;
        if (user is null)
        {
            user = User.Create(googleUser.FirstName, googleUser.LastName, googleUser.Email);
            isNewUser = true;
        }

        string accessToken = jwtService.GenerateToken(user);
        RefreshToken refreshToken = user.AddRefreshToken(refreshTokenFactory);

        if (isNewUser)
        {
            await userRepository.AddAsync(user, cancellationToken);
        }

        await userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(new SessionResponse(accessToken, refreshToken.Token, refreshToken.ExpiresAt));
    }
}
