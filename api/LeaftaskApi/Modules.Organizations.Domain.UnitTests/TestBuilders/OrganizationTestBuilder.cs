using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.Domain.UnitTests.TestBuilders;

#pragma warning disable CA1515
public sealed class OrganizationTestBuilder
#pragma warning restore CA1515
{
    private Guid _creatorUserId = Guid.NewGuid();
    private string _description = "Default organization description";
    private string _name = "Wayne Enterprises";
    private string _website = "https://wayneenterprises.com";

    private OrganizationTestBuilder() { }

    public static OrganizationTestBuilder AnOrganization() => new();

    public OrganizationTestBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public OrganizationTestBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public OrganizationTestBuilder WithWebsite(string website)
    {
        _website = website;
        return this;
    }

    public OrganizationTestBuilder WithCreatorUserId(Guid creatorUserId)
    {
        _creatorUserId = creatorUserId;
        return this;
    }

    public Organization Build() => Organization.Create(_name, _description, _website, _creatorUserId);
}
