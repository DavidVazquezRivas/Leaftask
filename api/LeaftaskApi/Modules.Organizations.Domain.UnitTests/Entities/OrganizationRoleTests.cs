using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Xunit;

namespace Modules.Organizations.Domain.UnitTests.Entities;

#pragma warning disable CA1515
public class OrganizationRoleTests
#pragma warning restore CA1515
{
    [Fact]
    public void Constructor_Should_InitializeRoleWithPermissions_When_PermissionsAreProvided()
    {
        // Arrange
        OrganizationPermission[] permissions =
        [
            new("organization.read", "Read organizations"),
            new("organization.edit", "Edit organizations")
        ];

        // Act
        OrganizationRole role = new("Owner", Guid.NewGuid(), permissions);

        // Assert
        role.Permissions.Should().HaveCount(2);
        role.Permissions.Should().OnlyContain(permission => permission.Level == PermissionLevel.None);
    }

    [Fact]
    public void Update_Should_ChangeRoleName()
    {
        // Arrange
        OrganizationRole role = new("Owner", Guid.NewGuid());

        // Act
        Result result = role.Update("Admin");

        // Assert
        result.IsSuccess.Should().BeTrue();
        role.Name.Should().Be("Admin");
    }

    [Fact]
    public void SetPermissionLevel_Should_ReturnFailure_When_PermissionDoesNotExist()
    {
        // Arrange
        OrganizationRole role = new("Owner", Guid.NewGuid());

        // Act
        Result result = role.SetPermissionLevel(Guid.NewGuid(), PermissionLevel.Full);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationRolePermissionNotFound);
    }
}
