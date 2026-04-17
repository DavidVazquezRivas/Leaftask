using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.Domain.UnitTests.TestBuilders;

#pragma warning disable CA1515
public sealed class OrganizationInvitationTestBuilder
#pragma warning restore CA1515
{
    private Guid _organizationId = Guid.NewGuid();
    private Guid _userId = Guid.NewGuid();

    private OrganizationInvitationTestBuilder() { }

    public static OrganizationInvitationTestBuilder AnInvitation() => new();

    public OrganizationInvitationTestBuilder WithOrganizationId(Guid organizationId)
    {
        _organizationId = organizationId;
        return this;
    }

    public OrganizationInvitationTestBuilder WithUserId(Guid userId)
    {
        _userId = userId;
        return this;
    }

    public OrganizationInvitation Build() => OrganizationInvitation.Create(_organizationId, _userId);
}
