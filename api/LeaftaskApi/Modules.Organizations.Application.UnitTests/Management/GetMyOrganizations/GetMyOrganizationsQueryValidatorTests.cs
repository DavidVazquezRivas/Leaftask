using FluentValidation.TestHelper;
using Modules.Organizations.Application.Management.GetMyOrganizations;
using Modules.Organizations.Application.UnitTests.TestBuilders;

namespace Modules.Organizations.Application.UnitTests.Management.GetMyOrganizations;

public class GetMyOrganizationsQueryValidatorTests
{
    private readonly GetMyOrganizationsQueryValidator _validator = new();

    [Fact]
    public void Validator_Should_NotHaveError_When_SortFieldIsValid()
    {
        // Arrange
        GetMyOrganizationsQuery query = GetMyOrganizationsQueryTestBuilder.AQuery()
            .WithSort(["name:asc", "createdAt:desc"])
            .Build();

        // Act
        TestValidationResult<GetMyOrganizationsQuery> result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Sort);
    }

    [Fact]
    public void Validator_Should_HaveError_When_SortFieldIsInvalid()
    {
        // Arrange
        GetMyOrganizationsQuery query = GetMyOrganizationsQueryTestBuilder.AQuery()
            .WithSort(["invalid:asc"])
            .Build();

        // Act
        TestValidationResult<GetMyOrganizationsQuery> result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Sort)
            .WithErrorMessage("The sort fields must be valid and unique.");
    }

    [Fact]
    public void Validator_Should_HaveError_When_SortFieldIsDuplicated()
    {
        // Arrange
        GetMyOrganizationsQuery query = GetMyOrganizationsQueryTestBuilder.AQuery()
            .WithSort(["name:asc", "name:desc"])
            .Build();

        // Act
        TestValidationResult<GetMyOrganizationsQuery> result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Sort)
            .WithErrorMessage("The sort fields must be valid and unique.");
    }
}
