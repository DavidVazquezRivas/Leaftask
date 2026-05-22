using FluentValidation.TestHelper;
using Modules.Organizations.Application.Invitations.Create;

namespace Modules.Organizations.Application.UnitTests.Invitations.Create;

public class CreateOrganizationInvitationCommandValidatorTests
{
    private readonly CreateOrganizationInvitationCommandValidator _validator = new();

    [Fact]
    public void Validator_Should_NotHaveError_When_RequestIsValid()
    {
        // Arrange
        CreateOrganizationInvitationCommand command = new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        // Act
        TestValidationResult<CreateOrganizationInvitationCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_Should_HaveError_When_RoleIdIsEmpty()
    {
        // Arrange
        CreateOrganizationInvitationCommand command = new(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty);

        // Act
        TestValidationResult<CreateOrganizationInvitationCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RoleId);
    }
}
