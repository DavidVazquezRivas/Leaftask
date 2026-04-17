using FluentAssertions;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.UnitTests.TestBuilders;
using Xunit;

namespace Modules.Organizations.Domain.UnitTests.Entities;

#pragma warning disable CA1515
public class OrganizationTests
#pragma warning restore CA1515
{
#pragma warning disable CA1822
    [Fact]
    public void Create_Should_InitializeOrganizationWithCorrectData()
    {
        // Arrange
        const string name = "Justice League";
        const string description = "Super hero organization";
        const string website = "https://justiceleague.org";
        Guid creatorUserId = Guid.NewGuid();

        // Act
        Organization organization = OrganizationTestBuilder.AnOrganization()
            .WithName(name)
            .WithDescription(description)
            .WithWebsite(website)
            .WithCreatorUserId(creatorUserId)
            .Build();

        // Assert
        organization.Id.Should().NotBe(Guid.Empty);
        organization.Name.Should().Be(name);
        organization.Description.Should().Be(description);
        organization.Website.Should().Be(website);
        organization.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        organization.Invitations.Should().ContainSingle();
        OrganizationInvitation invitation = organization.Invitations.Single();
        invitation.OrganizationId.Should().Be(organization.Id);
        invitation.UserId.Should().Be(creatorUserId);
        invitation.Status.Should().Be(InvitationStatus.Accepted);
        invitation.RespondedAt.Should().NotBeNull();
    }
#pragma warning restore CA1822
}
