using BuildingBlocks.Domain.Entities;

namespace Modules.Projects.Domain.Entities.Owner;

public sealed class OrganizationReadModel : Entity, IProjectOwner
{
    private OrganizationReadModel() { }

    public OrganizationReadModel(Guid id) => Id = id;

    public Guid Id { get; }
}
