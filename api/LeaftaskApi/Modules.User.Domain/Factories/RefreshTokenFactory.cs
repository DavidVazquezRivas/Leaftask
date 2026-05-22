using Modules.Users.Domain.Entities;

namespace Modules.Users.Domain.Factories;

public class RefreshTokenFactory(IRefreshTokenExpirationPolicy policy) : IRefreshTokenFactory
{
    public RefreshToken CreateForUser(Guid userId)
    {
        TimeSpan duration = policy.ExpirationDuration;

        return RefreshToken.Create(userId, duration);
    }
}
