using BuildingBlocks.Domain.Entities;

namespace Modules.Agents.Domain.Entities.Model;

public sealed class Model : Entity
{
    private Model() { }

    public Model(Guid id, string name, ModelProvider provider, string description, double cost)
    {
        Id = id;
        Name = name;
        Provider = provider;
        Description = description;
        Cost = cost;
    }

    public Guid Id { get; }
    public string Name { get; }

    public string Description { get; }

    // Input tokens cost estimation
    public double Cost { get; }
    public ModelProvider Provider { get; }
}
