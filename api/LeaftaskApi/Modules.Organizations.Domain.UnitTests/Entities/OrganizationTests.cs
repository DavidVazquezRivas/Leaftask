using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
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

        organization.Roles.Should().ContainSingle();
        OrganizationRole role = organization.Roles.Single();
        role.OrganizationId.Should().Be(organization.Id);
        role.Name.Should().Be("Owner");

        organization.Invitations.Should().ContainSingle();
        OrganizationInvitation invitation = organization.Invitations.Single();
        invitation.OrganizationId.Should().Be(organization.Id);
        invitation.OrganizationRoleId.Should().Be(role.Id);
        invitation.UserId.Should().Be(creatorUserId);
        invitation.Status.Should().Be(InvitationStatus.Accepted);
        invitation.RespondedAt.Should().NotBeNull();
    }

    [Fact]
    public void Update_Should_ChangeOnlyProvidedFields()
    {
        // Arrange
        Organization organization = OrganizationTestBuilder.AnOrganization().Build();

        // Act
        organization.Update(name: "Updated name", website: "https://updated.example.com");

        // Assert
        organization.Name.Should().Be("Updated name");
        organization.Description.Should().Be("Default organization description");
        organization.Website.Should().Be("https://updated.example.com");
    }

    [Fact]
    public void UpdateRole_Should_UpdateRoleName_And_Permissions()
    {
        // Arrange
        Organization organization = OrganizationTestBuilder.AnOrganization().Build();
        OrganizationPermission[] permissions =
        [
            new("organization.configure", "Configure Organization"),
            new("organization.invite_members", "Invite Members")
        ];

        OrganizationRole role = organization.AddRole("Manager", permissions);

        // Act
        Result result = organization.UpdateRole(role.Id, "Manager Updated", [(permissions[0].Id, PermissionLevel.Full)]);

        // Assert
        result.IsSuccess.Should().BeTrue();
        role.Name.Should().Be("Manager Updated");
        role.Permissions.Should().ContainSingle(permission => permission.OrganizationPermissionId == permissions[0].Id && permission.Level == PermissionLevel.Full);
        role.Permissions.Should().ContainSingle(permission => permission.OrganizationPermissionId == permissions[1].Id && permission.Level == PermissionLevel.None);
    }

    [Fact]
    public void UpdateMemberRole_Should_UpdateAcceptedMemberRole()
    {
        // Arrange
        Organization organization = OrganizationTestBuilder.AnOrganization().Build();
        OrganizationPermission[] permissions =
        [
            new("organization.configure", "Configure Organization")
        ];

        OrganizationRole newRole = organization.AddRole("Member", permissions);

        OrganizationInvitation invitation = organization.Invitations.Single();
        Guid memberId = invitation.UserId;

        // Act
        Result result = organization.UpdateMemberRole(memberId, newRole.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        invitation.OrganizationRoleId.Should().Be(newRole.Id);
    }

    [Fact]
    public void RemoveMember_Should_AbandonAcceptedInvitation()
    {
        // Arrange
        Organization organization = OrganizationTestBuilder.AnOrganization().Build();
        OrganizationInvitation invitation = organization.Invitations.Single();
        Guid memberId = invitation.UserId;

        // Act
        Result result = organization.RemoveMember(memberId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        invitation.Status.Should().Be(InvitationStatus.Abandoned);
        invitation.AbandonedAt.Should().NotBeNull();
    }

    [Fact]
    public void RemoveMember_Should_ReturnFailure_When_MemberDoesNotExist()
    {
        // Arrange
        Organization organization = OrganizationTestBuilder.AnOrganization().Build();

        // Act
        Result result = organization.RemoveMember(Guid.NewGuid());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationMemberNotFound);
    }

    [Fact]
    public void UpdateMemberRole_Should_ReturnFailure_When_MemberDoesNotExist()
    {
        // Arrange
        Organization organization = OrganizationTestBuilder.AnOrganization().Build();
        OrganizationRole role = organization.Roles.Single();

        // Act
        Result result = organization.UpdateMemberRole(Guid.NewGuid(), role.Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationMemberNotFound);
    }

    [Fact]
    public void UpdateRole_Should_ReturnFailure_When_RoleDoesNotExist()
    {
        // Arrange
        Organization organization = OrganizationTestBuilder.AnOrganization().Build();

        // Act
        Result result = organization.UpdateRole(Guid.NewGuid(), "Updated role");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationRoleNotFound);
    }

    [Fact]
    public void RemoveRole_Should_ReturnFailure_When_RoleDoesNotExist()
    {
        // Arrange
        Organization organization = OrganizationTestBuilder.AnOrganization().Build();

        // Act
        Result result = organization.RemoveRole(Guid.NewGuid());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationRoleNotFound);
    }
#pragma warning restore CA1822
}
