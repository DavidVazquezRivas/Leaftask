using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Owner;

namespace Modules.Projects.Domain.UnitTests.TestBuilders;

#pragma warning disable CA1515
public sealed class ProjectTestBuilder
#pragma warning restore CA1515
{
    private string _name = "Test Project";
    private string _abbreviation = "TST";
    private ProjectPrivacy _privacy = ProjectPrivacy.Public;
    private IProjectOwner _owner = new ProjectOwnerReference(Guid.NewGuid());
    private OwnerType _ownerType = OwnerType.User;

    private ProjectTestBuilder() { }

    public static ProjectTestBuilder AProject() => new();

    public ProjectTestBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ProjectTestBuilder WithAbbreviation(string abbreviation)
    {
        _abbreviation = abbreviation;
        return this;
    }

    public ProjectTestBuilder WithPrivacy(ProjectPrivacy privacy)
    {
        _privacy = privacy;
        return this;
    }

    public ProjectTestBuilder WithOwner(IProjectOwner owner, OwnerType ownerType)
    {
        _owner = owner;
        _ownerType = ownerType;
        return this;
    }

    public ProjectTestBuilder OwnedByUser(Guid userId)
    {
        _owner = new ProjectOwnerReference(userId);
        _ownerType = OwnerType.User;
        return this;
    }

    public ProjectTestBuilder OwnedByOrganization(Guid organizationId)
    {
        _owner = new ProjectOwnerReference(organizationId);
        _ownerType = OwnerType.Organization;
        return this;
    }

    public Project Build() => Project.Create(_name, _abbreviation, _privacy, _owner, _ownerType);
}
