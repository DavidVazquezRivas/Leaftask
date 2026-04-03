using BuildingBlocks.Domain.Specifications;
using Modules.Users.Domain.Entities;

namespace Modules.Users.Domain.Specifications;

public sealed class UserWithActiveRefreshTokensSpecification : BaseSpecification<User>
{
    public UserWithActiveRefreshTokensSpecification(Guid userId)
        : base(user => user.Id == userId) =>
        AddInclude(user => user.RefreshTokens
            .Where(rt => rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow));
}
