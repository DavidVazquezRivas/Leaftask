using Microsoft.EntityFrameworkCore;
using Modules.Agents.Domain.Entities.Model;

namespace Modules.Agents.DrivenInfrastructure.Persistence.Seeding;

public static class ModelSeeder
{
    private static readonly (Guid Id, string Name, string Description, double Cost, Guid ProviderId)[] Models =
    [
        (new Guid("b0000001-0000-0000-0000-000000000001"), "gpt-5.4-mini",
            "Fast and affordable OpenAi model, use this in most cases.", 0.75, ModelProviderSeeder.OpenAiProviderId),
        (new Guid("b0000001-0000-0000-0000-000000000002"), "gpt-5.4-nano",
            "Super light OpenAi model, use this for really simple tasks only. If the task has a minimum complexity go to mini",
            0.2, ModelProviderSeeder.OpenAiProviderId),
        (new Guid("b0000001-0000-0000-0000-000000000003"), "claude-sonnet-4-6",
            "Most potent OpenAi model, use it just for really critical tasks and orchestration. Whenever it's possible, avoid using this model",
            2.5, ModelProviderSeeder.OpenAiProviderId)
    ];

    public static async Task SeedAsync(AgentsDbContext dbContext, CancellationToken cancellationToken = default)
    {
        bool alreadySeeded = await dbContext.Models.AnyAsync(cancellationToken);
        if (alreadySeeded)
        {
            return;
        }

        foreach ((Guid id, string name, string description, double cost, Guid providerId) in Models)
        {
            ModelProvider provider = await dbContext.ModelProviders.FindAsync([providerId], cancellationToken)
                                     ?? throw new InvalidOperationException(
                                         $"ModelProvider {providerId} not found. Run ModelProviderSeeder first.");

            await dbContext.Models.AddAsync(
                new Model(id, name, provider, description, cost), cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
