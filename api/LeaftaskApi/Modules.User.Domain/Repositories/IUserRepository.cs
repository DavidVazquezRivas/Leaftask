using BuildingBlocks.Domain.Specifications;
using Modules.Users.Domain.Entities;

namespace Modules.Users.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetBySpecAsync(ISpecification<User> spec, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
