using Modules.Organizations.Application.Management.Create;

namespace Modules.Organizations.Application.UnitTests.TestBuilders;

internal sealed class CreateOrganizationCommandTestBuilder
{
    private string _description = "Default organization description";
    private string _name = "Wayne Enterprises";
    private string _website = "https://wayneenterprises.com";

    private CreateOrganizationCommandTestBuilder() { }

    public static CreateOrganizationCommandTestBuilder ACommand() => new();

    public CreateOrganizationCommandTestBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CreateOrganizationCommandTestBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public CreateOrganizationCommandTestBuilder WithWebsite(string website)
    {
        _website = website;
        return this;
    }

    public CreateOrganizationCommand Build() => new(_name, _description, _website);
}
