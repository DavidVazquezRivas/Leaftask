using FluentValidation.TestHelper;
using Modules.Organizations.Application.Roles.Create;
using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.Application.UnitTests.Roles.Create;

public class CreateOrganizationRoleCommandValidatorTests
{
    private readonly CreateOrganizationRoleCommandValidator _validator = new();

    [Fact]
    public void Validator_Should_NotHaveError_When_RequestIsValid()
    {
        // Arrange
        CreateOrganizationRoleCommand command = new(
            Guid.NewGuid(),
            "Leaftask Admin",
            [new CreateOrganizationRolePermissionInput(Guid.NewGuid(), PermissionLevel.Full)]);

        // Act
        TestValidationResult<CreateOrganizationRoleCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_Should_HaveError_When_PermissionsContainDuplicates()
    {
        // Arrange
        Guid permissionId = Guid.NewGuid();
        CreateOrganizationRoleCommand command = new(
            Guid.NewGuid(),
            "Leaftask Admin",
            [
                new CreateOrganizationRolePermissionInput(permissionId, PermissionLevel.Full),
                new CreateOrganizationRolePermissionInput(permissionId, PermissionLevel.Supervised)
            ]);

        // Act
        TestValidationResult<CreateOrganizationRoleCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Permissions)
            .WithErrorMessage("Permissions must be unique.");
    }
}
