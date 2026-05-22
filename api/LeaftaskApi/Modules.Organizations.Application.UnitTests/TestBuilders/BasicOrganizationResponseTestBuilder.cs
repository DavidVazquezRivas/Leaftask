using Modules.Organizations.Application.Management;

namespace Modules.Organizations.Application.UnitTests.TestBuilders;

internal sealed class BasicOrganizationResponseTestBuilder
{
    private int _activeProjects;
    private DateTime _createdAt = DateTime.UtcNow;
    private int _customRoles;
    private string _description = "Default organization description";
    private Guid _id = Guid.NewGuid();
    private string _name = "Wayne Enterprises";
    private int _totalMembers;
    private string _website = "https://wayne-enterprises.com";

    private BasicOrganizationResponseTestBuilder() { }

    public static BasicOrganizationResponseTestBuilder AResponse() => new();

    public BasicOrganizationResponseTestBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public BasicOrganizationResponseTestBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public BasicOrganizationResponseTestBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public BasicOrganizationResponseTestBuilder WithWebsite(string website)
    {
        _website = website;
        return this;
    }

    public BasicOrganizationResponseTestBuilder WithTotalMembers(int totalMembers)
    {
        _totalMembers = totalMembers;
        return this;
    }

    public BasicOrganizationResponseTestBuilder WithActiveProjects(int activeProjects)
    {
        _activeProjects = activeProjects;
        return this;
    }

    public BasicOrganizationResponseTestBuilder WithCustomRoles(int customRoles)
    {
        _customRoles = customRoles;
        return this;
    }

    public BasicOrganizationResponseTestBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public BasicOrganizationResponse Build() =>
        new(_id, _name, _description, _website, _totalMembers, _activeProjects, _customRoles, _createdAt);
}
