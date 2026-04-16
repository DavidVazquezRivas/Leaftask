using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Errors;
using Modules.Users.Domain.Events;
using Modules.Users.Domain.Factories;
using Modules.Users.Domain.UnitTests.TestBuilders;
using NSubstitute;
using Xunit;

namespace Modules.Users.Domain.UnitTests.Entities;

public class UserTests
{
    [Fact]
    public void Create_Should_InitializeUserWithCorrectData()
    {
        // Arrange
        const string firstName = "Clark";
        const string lastName = "Kent";
        const string email = "clark@dailyplanet.com";

        // Act
        User user = UserTestBuilder.AUser()
            .WithFirstName(firstName)
            .WithLastName(lastName)
            .WithEmail(email)
            .Build();

        // Assert
        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
        user.Email.Should().Be(email);
        user.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<UserCreatedDomainEvent>();
    }

    [Fact]
    public void AddRefreshToken_Should_AppendTokenToUserCollection()
    {
        // Arrange
        User user = UserTestBuilder.AUser().Build();
        RefreshToken refreshToken = RefreshToken.Create(user.Id, TimeSpan.FromDays(1));
        IRefreshTokenFactory? factoryMock = Substitute.For<IRefreshTokenFactory>();
        factoryMock.CreateForUser(user.Id).Returns(refreshToken);

        // Act
        RefreshToken result = user.AddRefreshToken(factoryMock);

        // Assert
        result.Should().BeSameAs(refreshToken);
        user.RefreshTokens.Should().Contain(refreshToken);
    }

    [Fact]
    public void RotateRefreshToken_Should_ReturnFailure_When_CurrentTokenIsNotFound()
    {
        // Arrange
        User user = UserTestBuilder.AUser().Build();
        IRefreshTokenFactory? factoryMock = Substitute.For<IRefreshTokenFactory>();

        // Act
        Result<RefreshToken> result = user.RotateRefreshToken("invalid-token", factoryMock);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidRefreshToken);
    }

    [Fact]
    public void RotateRefreshToken_Should_InvalidateOldTokenAndAddNextOne_When_TokenIsValid()
    {
        // Arrange
        User user = UserTestBuilder.AUser().Build();
        IRefreshTokenFactory? factoryMock = Substitute.For<IRefreshTokenFactory>();

        RefreshToken oldToken = RefreshToken.Create(user.Id, TimeSpan.FromDays(1));
        RefreshToken newToken = RefreshToken.Create(user.Id, TimeSpan.FromDays(1));

        factoryMock.CreateForUser(user.Id).Returns(oldToken, newToken);

        user.AddRefreshToken(factoryMock); // Adds oldToken

        // Act
        Result<RefreshToken> result = user.RotateRefreshToken(oldToken.Token, factoryMock);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(newToken);
        oldToken.IsRevoked.Should().BeTrue();
        user.RefreshTokens.Should().HaveCount(2);
    }

    [Fact]
    public void RevokeAllRefreshTokens_Should_DeactivateEveryTokenInCollection()
    {
        // Arrange
        User user = UserTestBuilder.AUser().Build();
        IRefreshTokenFactory? factoryMock = Substitute.For<IRefreshTokenFactory>();

        RefreshToken token1 = RefreshToken.Create(user.Id, TimeSpan.FromDays(1));
        RefreshToken token2 = RefreshToken.Create(user.Id, TimeSpan.FromDays(1));

        factoryMock.CreateForUser(user.Id).Returns(token1, token2);

        user.AddRefreshToken(factoryMock);
        user.AddRefreshToken(factoryMock);

        // Act
        user.RevokeAllRefreshTokens();

        // Assert
        user.RefreshTokens.Should().OnlyContain(t => t.IsRevoked);
        user.RefreshTokens.Should().OnlyContain(t => !t.IsValid);
    }
}
