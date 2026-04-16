using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Xunit;

namespace BuildingBlocks.Domain.UnitTests.Result;

public class ResultTests
{
    private static readonly Error TestError = new("Test.Error", "This is a test error", 400);

    [Fact]
    public void Constructor_Should_ThrowArgumentException_When_SuccessWithRealError()
    {
        // Act
        Action act = () => _ = new Domain.Result.Result(true, TestError);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid error (Parameter 'error')");
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentException_When_FailureWithNoError()
    {
        // Act
        Action act = () => _ = new Domain.Result.Result(false, Error.None);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid error (Parameter 'error')");
    }

    [Fact]
    public void Success_Should_ReturnSuccessResult()
    {
        // Act
        Domain.Result.Result result = Domain.Result.Result.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_Should_ReturnFailureResult()
    {
        // Act
        Domain.Result.Result result = Domain.Result.Result.Failure(TestError);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TestError);
    }

    [Fact]
    public void GenericSuccess_Should_ReturnSuccessResultWithValue()
    {
        // Arrange
        const string expectedValue = "Hello World";

        // Act
        Result<string> result = Domain.Result.Result.Success(expectedValue);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expectedValue);
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void GenericFailure_Should_ReturnFailureResultWithoutValue()
    {
        // Act
        Result<string> result = Domain.Result.Result.Failure<string>(TestError);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(TestError);
    }

    [Fact]
    public void Value_Should_ThrowInvalidOperationException_When_ResultIsFailure()
    {
        // Arrange
        Result<string> result = Domain.Result.Result.Failure<string>(TestError);

        // Act
        Func<string> act = () => result.Value;

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot access the value of a failure result.");
    }

    [Fact]
    public void ImplicitOperator_Should_ReturnSuccessResult_When_ValueIsNotNull()
    {
        // Arrange
        const string value = "Valid string";

        // Act
        Result<string> result = value;

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(value);
    }

    [Fact]
    public void ImplicitOperator_Should_ReturnFailureResult_When_ValueIsNull()
    {
        // Arrange
        string? nullValue = null;

        // Act
        Result<string> result = nullValue;

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(Error.NullValue);

        Func<string> act = () => result.Value;
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ValidationFailure_Should_ReturnFailureResult()
    {
        // Act
        Result<string> result = Result<string>.ValidationFailure(TestError);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(TestError);
    }
}
