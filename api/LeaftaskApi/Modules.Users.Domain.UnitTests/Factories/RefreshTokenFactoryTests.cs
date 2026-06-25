using FluentAssertions;
using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Factories;
using NSubstitute;
using Xunit;

namespace Modules.Users.Domain.UnitTests.Factories;

public class RefreshTokenFactoryTests
{
    [Fact]
    public void CreateForUser_Should_ReturnRefreshToken_WithCorrectUserId()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        TimeSpan duration = TimeSpan.FromDays(7);

        IRefreshTokenExpirationPolicy policyMock = Substitute.For<IRefreshTokenExpirationPolicy>();
        policyMock.ExpirationDuration.Returns(duration);

        RefreshTokenFactory factory = new(policyMock);

        // Act
        RefreshToken token = factory.CreateForUser(userId);

        // Assert
        token.Should().NotBeNull();
        token.UserId.Should().Be(userId);
    }

    [Fact]
    public void CreateForUser_Should_ReturnToken_WithNonEmptyTokenString()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        IRefreshTokenExpirationPolicy policyMock = Substitute.For<IRefreshTokenExpirationPolicy>();
        policyMock.ExpirationDuration.Returns(TimeSpan.FromDays(1));

        RefreshTokenFactory factory = new(policyMock);

        // Act
        RefreshToken token = factory.CreateForUser(userId);

        // Assert
        token.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void CreateForUser_Should_UseExpirationDurationFromPolicy()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        TimeSpan expectedDuration = TimeSpan.FromDays(30);

        IRefreshTokenExpirationPolicy policyMock = Substitute.For<IRefreshTokenExpirationPolicy>();
        policyMock.ExpirationDuration.Returns(expectedDuration);

        RefreshTokenFactory factory = new(policyMock);
        DateTime before = DateTime.UtcNow;

        // Act
        RefreshToken token = factory.CreateForUser(userId);

        // Assert
        token.ExpiresAt.Should().BeCloseTo(before.Add(expectedDuration), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void CreateForUser_Should_ReturnDistinctTokens_On_SuccessiveCalls()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        IRefreshTokenExpirationPolicy policyMock = Substitute.For<IRefreshTokenExpirationPolicy>();
        policyMock.ExpirationDuration.Returns(TimeSpan.FromDays(7));

        RefreshTokenFactory factory = new(policyMock);

        // Act
        RefreshToken token1 = factory.CreateForUser(userId);
        RefreshToken token2 = factory.CreateForUser(userId);

        // Assert
        token1.Token.Should().NotBe(token2.Token);
    }
}
