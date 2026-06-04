namespace Modules.Agents.Application.Bootstrap;

public interface IBootstrapAgentService
{
    Task<BootstrapAgentResult?> GenerateAsync(
        string instructions,
        IReadOnlyList<AvailableModelDto> availableModels,
        IReadOnlyList<string> availableEventTypes,
        CancellationToken cancellationToken = default);
}
