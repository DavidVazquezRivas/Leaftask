using FluentValidation.TestHelper;
using Modules.Organizations.Application.Members.GetMembers;

namespace Modules.Organizations.Application.UnitTests.Members.GetMembers;

public class GetOrganizationMembersQueryValidatorTests
{
    private readonly GetOrganizationMembersQueryValidator _validator = new();

    [Fact]
    public void Validator_Should_NotHaveError_When_RequestIsValid()
    {
        // Arrange
        GetOrganizationMembersQuery query = new()
        {
            OrganizationId = Guid.NewGuid(),
            Limit = 10,
            Sort = ["name:asc", "id:asc"]
        };

        // Act
        TestValidationResult<GetOrganizationMembersQuery> result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validator_Should_HaveError_When_SortFieldsAreDuplicated()
    {
        // Arrange
        GetOrganizationMembersQuery query = new()
        {
            OrganizationId = Guid.NewGuid(),
            Limit = 10,
            Sort = ["name:asc", "name:desc"]
        };

        // Act
        TestValidationResult<GetOrganizationMembersQuery> result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Sort)
            .WithErrorMessage("The sort fields must be valid and unique.");
    }
}
