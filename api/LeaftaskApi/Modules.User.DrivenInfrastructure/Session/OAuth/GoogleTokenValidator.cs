using BuildingBlocks.Domain.Result;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Modules.Users.Application.Session.OAuth.Google;
using Modules.Users.Domain.Errors;

namespace Modules.Users.DrivenInfrastructure.Session.OAuth;

public class GoogleTokenValidator(IOptions<GoogleAuthOptions> options) : IGoogleTokenValidator
{
    private readonly GoogleAuthOptions _options = options.Value;

    public async Task<Result<GoogleUserPayload>> ValidateAsync(string idToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            GoogleJsonWebSignature.ValidationSettings settings = new()
            {
                Audience = [_options.ClientId]
            };

            GoogleJsonWebSignature.Payload? payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            GoogleUserPayload googleUser = new(
                payload.Email,
                payload.GivenName ?? "Unknown",
                payload.FamilyName ?? "");

            return Result.Success(googleUser);
        }
        catch (InvalidJwtException)
        {
            return Result.Failure<GoogleUserPayload>(UserErrors.InvalidGoogleToken);
        }
    }
}
