using FluentValidation.TestHelper;
using Modules.Organizations.Application.Members.UpdateRole;

namespace Modules.Organizations.Application.UnitTests.Members.UpdateRole;

public class UpdateOrganizationMemberRoleCommandValidatorTests
{
    private readonly UpdateOrganizationMemberRoleCommandValidator _validator = new();

    [Fact]
    public void Validator_Should_NotHaveError_When_RequestIsValid()
    {
        // Arrange
        UpdateOrganizationMemberRoleCommand command = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Act
        TestValidationResult<UpdateOrganizationMemberRoleCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_Should_HaveError_When_RoleIdIsEmpty()
    {
        // Arrange
        UpdateOrganizationMemberRoleCommand command = new(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty);

        // Act
        TestValidationResult<UpdateOrganizationMemberRoleCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RoleId);
    }
}
