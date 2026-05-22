using BuildingBlocks.Domain.Result;

namespace Modules.Users.Application.Session.OAuth.Google;

public interface IGoogleTokenValidator
{
    Task<Result<GoogleUserPayload>> ValidateAsync(string idToken, CancellationToken cancellationToken = default);
}
