using BuildingBlocks.Application.Queries;
using BuildingBlocks.Application.Validation;
using FluentValidation.TestHelper;

namespace BuildingBlocks.Application.UnitTests.Validation;

// Dummy classes to implement the interfaces and abstract classes
#pragma warning disable S2094 // Classes should not be empty
internal sealed record TestPaginatedResponse;
#pragma warning restore S2094 // Classes should not be empty

internal sealed record TestPaginatedQuery : IPaginatedQuery<TestPaginatedResponse>
{
    public IReadOnlyCollection<string> Sort { get; init; } = [];
    public int Limit { get; init; }
    public string? Cursor { get; init; }
}

internal sealed class TestPaginatedQueryValidator : PaginatedQueryValidator<TestPaginatedQuery, TestPaginatedResponse>;

public class PaginatedQueryValidatorTests
{
    private readonly TestPaginatedQueryValidator _validator = new();

    // --- LIMIT TESTS ---

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void Limit_Should_NotHaveError_When_WithinValidRange(int validLimit)
    {
        // Arrange
        TestPaginatedQuery query = new() { Limit = validLimit };

        // Act
        TestValidationResult<TestPaginatedQuery>? result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Limit);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public void Limit_Should_HaveError_When_OutsideValidRange(int invalidLimit)
    {
        // Arrange
        TestPaginatedQuery query = new() { Limit = invalidLimit };

        // Act
        TestValidationResult<TestPaginatedQuery>? result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Limit)
            .WithErrorMessage("Limit must be between 1 and 100.");
    }

    // --- CURSOR TESTS ---

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Cursor_Should_NotHaveError_When_NullOrEmpty(string? emptyCursor)
    {
        // Arrange
        TestPaginatedQuery query = new() { Limit = 10, Cursor = emptyCursor };

        // Act
        TestValidationResult<TestPaginatedQuery>? result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Cursor);
    }

    [Fact]
    public void Cursor_Should_NotHaveError_When_ValidGuid()
    {
        // Arrange
        TestPaginatedQuery query = new() { Limit = 10, Cursor = Guid.NewGuid().ToString() };

        // Act
        TestValidationResult<TestPaginatedQuery>? result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Cursor);
    }

    [Theory]
    [InlineData("not-a-guid")]
    [InlineData("12345")]
    public void Cursor_Should_HaveError_When_InvalidGuid(string invalidCursor)
    {
        // Arrange
        TestPaginatedQuery query = new() { Limit = 10, Cursor = invalidCursor };

        // Act
        TestValidationResult<TestPaginatedQuery>? result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Cursor)
            .WithErrorMessage("Cursor must be a valid Guid.");
    }

    // --- SORT TESTS ---

    [Fact]
    public void Sort_Should_NotHaveError_When_ListIsEmpty()
    {
        // Arrange
        TestPaginatedQuery query = new() { Limit = 10, Sort = [] };

        // Act
        TestValidationResult<TestPaginatedQuery>? result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Sort);
    }

    [Theory]
    [InlineData("name:asc")]
    [InlineData("createdAt:desc")]
    [InlineData("user_id:asc")]
    [InlineData("VALUE123:desc")]
    public void Sort_Should_NotHaveError_When_FormatIsValid(string validSort)
    {
        // Arrange
        TestPaginatedQuery query = new() { Limit = 10, Sort = [validSort] };

        // Act
        TestValidationResult<TestPaginatedQuery>? result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Sort);
    }

    [Theory]
    [InlineData("name")] // Missing direction
    [InlineData("name:")] // Missing direction value
    [InlineData(":asc")] // Missing field name
    [InlineData("name:ascending")] // Invalid direction word
    [InlineData("name-asc")] // Wrong separator
    [InlineData("na-me:asc")] // Invalid characters in field name (hyphen)
    public void Sort_Should_HaveError_When_FormatIsInvalid(string invalidSort)
    {
        // Arrange
        TestPaginatedQuery query = new() { Limit = 10, Sort = [invalidSort] };

        // Act
        TestValidationResult<TestPaginatedQuery>? result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Sort)
            .WithErrorMessage("The sort format must be 'field:asc' or 'field:desc'.");
    }
}
