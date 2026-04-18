using FluentValidation.TestHelper;
using Modules.Organizations.Application.Roles.Patch;
using Modules.Organizations.Domain.Entities;

namespace Modules.Organizations.Application.UnitTests.Roles.Patch;

public class PatchOrganizationRoleCommandValidatorTests
{
    private readonly PatchOrganizationRoleCommandValidator _validator = new();

    [Fact]
    public void Validator_Should_NotHaveError_When_RequestHasNameOnly()
    {
        // Arrange
        PatchOrganizationRoleCommand command = new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Leaftask Admin Updated",
            null);

        // Act
        TestValidationResult<PatchOrganizationRoleCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_Should_NotHaveError_When_RequestHasPermissionsOnly()
    {
        // Arrange
        PatchOrganizationRoleCommand command = new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            [new PatchOrganizationRolePermissionInput(Guid.NewGuid(), PermissionLevel.Full)]);

        // Act
        TestValidationResult<PatchOrganizationRoleCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_Should_HaveError_When_RequestHasNoFields()
    {
        // Arrange
        PatchOrganizationRoleCommand command = new(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            null);

        // Act
        TestValidationResult<PatchOrganizationRoleCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("At least one field must be provided.");
    }
}
