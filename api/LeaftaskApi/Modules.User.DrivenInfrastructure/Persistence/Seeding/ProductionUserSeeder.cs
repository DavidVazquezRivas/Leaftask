using Microsoft.EntityFrameworkCore;

namespace Modules.Users.DrivenInfrastructure.Persistence.Seeding;

public sealed class ProductionUserSeeder : IUserSeederStrategy
{
    public async Task SeedAsync(UserDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Users.AnyAsync(cancellationToken))
        {
            // Logic to seed production base data
        }
    }
}
