using FluentValidation.TestHelper;
using Modules.Users.Application.Management.GetAll;

namespace Modules.Users.Application.UnitTests.Management.GetAll;

public class GetAllUsersQueryValidatorTests
{
    private readonly GetAllUsersQueryValidator _validator = new();

    [Fact]
    public void Validator_Should_HaveError_When_SearchExceeds50Characters()
    {
        // Arrange
        string longSearch = new('a', 51);
        GetAllUsersQuery query = GetAllUsersQueryTestBuilder.AQuery()
            .WithSearch(longSearch)
            .Build();

        // Act
        TestValidationResult<GetAllUsersQuery>? result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Search)
            .WithErrorMessage("The search cannot exceed 50 characters.");
    }

    [Fact]
    public void Validator_Should_NotHaveError_When_SearchIsWithinLimits()
    {
        // Arrange
        GetAllUsersQuery query = GetAllUsersQueryTestBuilder.AQuery()
            .WithSearch("Valid Search")
            .Build();

        // Act
        TestValidationResult<GetAllUsersQuery>? result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Search);
    }

    [Fact]
    public void Validator_Should_InheritBasePaginationRules()
    {
        // Arrange - Limit 0 is invalid according to PaginatedQueryValidator
        GetAllUsersQuery query = GetAllUsersQueryTestBuilder.AQuery()
            .WithLimit(0)
            .Build();

        // Act
        TestValidationResult<GetAllUsersQuery>? result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Limit);
    }

    [Fact]
    public void Validator_Should_NotHaveError_When_SortFieldIsValid()
    {
        // Arrange
        GetAllUsersQuery query = GetAllUsersQueryTestBuilder.AQuery()
            .WithSort(["email:asc", "firstName:desc"])
            .Build();

        // Act
        TestValidationResult<GetAllUsersQuery> result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Sort);
    }

    [Fact]
    public void Validator_Should_HaveError_When_SortFieldIsInvalid()
    {
        // Arrange
        GetAllUsersQuery query = GetAllUsersQueryTestBuilder.AQuery()
            .WithSort(["invalid:asc"])
            .Build();

        // Act
        TestValidationResult<GetAllUsersQuery> result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Sort)
            .WithErrorMessage("The sort fields must be valid and unique.");
    }

    [Fact]
    public void Validator_Should_HaveError_When_SortFieldIsDuplicated()
    {
        // Arrange
        GetAllUsersQuery query = GetAllUsersQueryTestBuilder.AQuery()
            .WithSort(["email:asc", "email:desc"])
            .Build();

        // Act
        TestValidationResult<GetAllUsersQuery> result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Sort)
            .WithErrorMessage("The sort fields must be valid and unique.");
    }
}
