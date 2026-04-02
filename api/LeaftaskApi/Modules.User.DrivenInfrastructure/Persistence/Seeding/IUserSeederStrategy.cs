namespace Modules.Users.DrivenInfrastructure.Persistence.Seeding;

public interface IUserSeederStrategy
{
    Task SeedAsync(UserDbContext dbContext, CancellationToken cancellationToken = default);
}
