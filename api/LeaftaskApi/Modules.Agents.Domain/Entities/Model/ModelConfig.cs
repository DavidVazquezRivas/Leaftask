using BuildingBlocks.Domain.Entities;

namespace Modules.Agents.Domain.Entities.Model;

public sealed class ModelConfig : Entity
{
    private ModelConfig() { }

    public ModelConfig(Guid id, Model model, double temperature, int maxTokens)
    {
        Id = id;
        Model = model;
        Temperature = temperature;
        MaxTokens = maxTokens;
    }

    public Guid Id { get; }
    public Model Model { get; }
    public double Temperature { get; }
    public int MaxTokens { get; }
}
