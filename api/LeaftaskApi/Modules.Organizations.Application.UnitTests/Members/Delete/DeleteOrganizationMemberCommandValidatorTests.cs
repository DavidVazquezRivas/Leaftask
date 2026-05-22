using FluentValidation.TestHelper;
using Modules.Organizations.Application.Members.Delete;

namespace Modules.Organizations.Application.UnitTests.Members.Delete;

public class DeleteOrganizationMemberCommandValidatorTests
{
    private readonly DeleteOrganizationMemberCommandValidator _validator = new();

    [Fact]
    public void Validator_Should_NotHaveError_When_RequestIsValid()
    {
        // Arrange
        DeleteOrganizationMemberCommand command = new(Guid.NewGuid(), Guid.NewGuid());

        // Act
        TestValidationResult<DeleteOrganizationMemberCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_Should_HaveError_When_MemberIdIsEmpty()
    {
        // Arrange
        DeleteOrganizationMemberCommand command = new(Guid.NewGuid(), Guid.Empty);

        // Act
        TestValidationResult<DeleteOrganizationMemberCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MemberId);
    }
}
