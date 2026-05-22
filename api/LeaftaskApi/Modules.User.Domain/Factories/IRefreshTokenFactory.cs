using Modules.Users.Domain.Entities;

namespace Modules.Users.Domain.Factories;

public interface IRefreshTokenFactory
{
    RefreshToken CreateForUser(Guid userId);
}
