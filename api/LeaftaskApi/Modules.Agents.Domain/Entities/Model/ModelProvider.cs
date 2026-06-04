using BuildingBlocks.Domain.Entities;

namespace Modules.Agents.Domain.Entities.Model;

public sealed class ModelProvider : Entity
{
    private ModelProvider() { }

    public ModelProvider(Guid id, string name, string token)
    {
        Id = id;
        Name = name;
        Token = token;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string Token { get; }
}
