using Modules.Organizations.Application.Management.GetDetails;

namespace Modules.Organizations.Application.UnitTests.Management.GetDetails;

internal sealed class GetOrganizationDetailsQueryTestBuilder
{
    private Guid _id = Guid.NewGuid();

    private GetOrganizationDetailsQueryTestBuilder() { }

    public static GetOrganizationDetailsQueryTestBuilder AQuery() => new();

    public GetOrganizationDetailsQueryTestBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public GetOrganizationDetailsQuery Build() => new(_id);
}
