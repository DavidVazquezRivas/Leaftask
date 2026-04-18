using Modules.Organizations.Application.Management.GetMyOrganizations;

namespace Modules.Organizations.Application.UnitTests.TestBuilders;

internal sealed class SimpleOrganizationDtoTestBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _name = "Wayne Enterprises";

    private SimpleOrganizationDtoTestBuilder() { }

    public static SimpleOrganizationDtoTestBuilder ADto() => new();

    public SimpleOrganizationDtoTestBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public SimpleOrganizationDtoTestBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public SimpleOrganizationDto Build() => new(_id, _name);
}
