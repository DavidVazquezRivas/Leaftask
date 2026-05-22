namespace Modules.Organizations.DrivenInfrastructure.Persistence.Seeding;

public interface IOrganizationSeederStrategy
{
    Task SeedAsync(OrganizationDbContext dbContext, CancellationToken cancellationToken = default);
}
