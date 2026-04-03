using BuildingBlocks.Domain.Specifications;
using Microsoft.EntityFrameworkCore;
using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Repositories;
using Modules.Users.DrivenInfrastructure.Persistence;

namespace Modules.Users.DrivenInfrastructure.Repositories;

public class UserRepository(UserDbContext dbContext) : IUserRepository
{
    public async Task<User?> GetBySpecAsync(ISpecification<User> spec, CancellationToken cancellationToken = default)
    {
        IQueryable<User> query = dbContext.Set<User>();

        query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

        return await query.FirstOrDefaultAsync(spec.Criteria, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await dbContext.Users
            .SingleOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<User?>
        GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default) =>
        await dbContext.Users
            .Include(u =>
                u.RefreshTokens.Where(rt =>
                    rt.Token == refreshToken)) // Fetch only the relevant refresh token to avoid N+1
            .SingleOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken), cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        await dbContext.Users.AddAsync(user, cancellationToken);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
