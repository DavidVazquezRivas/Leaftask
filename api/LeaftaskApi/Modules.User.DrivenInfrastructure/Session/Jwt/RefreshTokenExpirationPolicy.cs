using Microsoft.Extensions.Options;
using Modules.Users.Domain.Factories;

namespace Modules.Users.DrivenInfrastructure.Session.Jwt;

public class RefreshTokenExpirationPolicy(IOptions<JwtOptions> options) : IRefreshTokenExpirationPolicy
{
    public TimeSpan ExpirationDuration => TimeSpan.FromDays(options.Value.RefreshTokenExpirationInDays);
}
