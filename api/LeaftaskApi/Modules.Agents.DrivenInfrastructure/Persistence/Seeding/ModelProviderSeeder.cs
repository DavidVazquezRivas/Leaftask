using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Modules.Agents.Domain.Entities.Model;

namespace Modules.Agents.DrivenInfrastructure.Persistence.Seeding;

public static class ModelProviderSeeder
{
    public static readonly Guid OpenAiProviderId = new("a0000001-0000-0000-0000-000000000001");
    public static readonly Guid AnthropicProviderId = new("a0000001-0000-0000-0000-000000000002");

    public static async Task SeedAsync(
        AgentsDbContext dbContext,
        IConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        string openAiToken = configuration["Modules:Agents:Providers:OpenAi:ApiKey"] ?? string.Empty;
        string anthropicToken = configuration["Modules:Agents:Providers:Anthropic:ApiKey"] ?? string.Empty;

        await UpsertProviderAsync(dbContext, OpenAiProviderId, "OpenAI", openAiToken, cancellationToken);
        await UpsertProviderAsync(dbContext, AnthropicProviderId, "Anthropic", anthropicToken, cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task UpsertProviderAsync(
        AgentsDbContext dbContext,
        Guid id,
        string name,
        string token,
        CancellationToken cancellationToken)
    {
        ModelProvider? existing = await dbContext.ModelProviders
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (existing is null)
        {
            await dbContext.ModelProviders.AddAsync(new ModelProvider(id, name, token), cancellationToken);
        }
        else
        {
            await dbContext.ModelProviders
                .Where(p => p.Id == id)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.Token, token), cancellationToken);
        }
    }
}
