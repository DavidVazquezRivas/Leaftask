using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Xunit;

namespace BuildingBlocks.Domain.UnitTests.Result;

public class ErrorTests
{
    [Fact]
    public void Constructor_Should_AssignPropertiesCorrectly()
    {
        // Arrange
        const string expectedCode = "Test.Code";
        const string expectedDescription = "Test description";
        const int expectedStatusCode = 400;

        // Act
        Error error = new(expectedCode, expectedDescription, expectedStatusCode);

        // Assert
        error.Code.Should().Be(expectedCode);
        error.Description.Should().Be(expectedDescription);
        error.StatusCode.Should().Be(expectedStatusCode);
    }

    [Fact]
    public void None_Should_ReturnExpectedEmptyError()
    {
        // Act
        Error error = Error.None;

        // Assert
        error.Code.Should().BeEmpty();
        error.Description.Should().BeEmpty();
        error.StatusCode.Should().Be(200);
    }

    [Fact]
    public void NullValue_Should_ReturnExpectedNullError()
    {
        // Act
        Error error = Error.NullValue;

        // Assert
        error.Code.Should().Be("General.Null");
        error.Description.Should().Be("Null value was provided");
        error.StatusCode.Should().Be(500);
    }
}
