using Microsoft.EntityFrameworkCore;
using Modules.Users.Domain.Entities;

namespace Modules.Users.DrivenInfrastructure.Persistence.Seeding;

public sealed class DevelopmentUserSeeder : IUserSeederStrategy
{
    public async Task SeedAsync(UserDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (!await dbContext.Users.AnyAsync(cancellationToken))
        {
            User[] users =
            [
                User.Create("Admin", "Developer", "admin@dev.com"),
                User.Create("John", "Doe", "john@dev.com"),
                User.Create("Jane", "Smith", "jane@dev.com")
            ];

            await dbContext.Users.AddRangeAsync(users, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
