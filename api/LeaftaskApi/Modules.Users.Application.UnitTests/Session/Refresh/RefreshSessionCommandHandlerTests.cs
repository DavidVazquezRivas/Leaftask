using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Users.Application.Session;
using Modules.Users.Application.Session.Jwt;
using Modules.Users.Application.Session.Refresh;
using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Errors;
using Modules.Users.Domain.Factories;
using Modules.Users.Domain.Repositories;
using Modules.Users.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Users.Application.UnitTests.Session.Refresh;

public class RefreshSessionCommandHandlerTests
{
    private readonly RefreshSessionCommandHandler _handler;
    private readonly IJwtService _jwtServiceMock;
    private readonly IRefreshTokenFactory _refreshTokenFactoryMock;
    private readonly IUserRepository _userRepositoryMock;

    public RefreshSessionCommandHandlerTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _jwtServiceMock = Substitute.For<IJwtService>();
        _refreshTokenFactoryMock = Substitute.For<IRefreshTokenFactory>();

        _handler = new RefreshSessionCommandHandler(
            _userRepositoryMock,
            _jwtServiceMock,
            _refreshTokenFactoryMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserIsNotFound()
    {
        // Arrange
        RefreshSessionCommand command = RefreshSessionCommandTestBuilder.ACommand().Build();

        _userRepositoryMock.GetByRefreshTokenAsync(command.RefreshToken, Arg.Any<CancellationToken>())
            .Returns((User)null!);

        // Act
        Result<SessionResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidRefreshToken);

        await _userRepositoryMock.DidNotReceiveWithAnyArgs()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_TokenRotationFails()
    {
        // Arrange
        User user = UserTestBuilder.AUser().Build();

        // We simulate a rotation failure by passing a command with a token 
        // that does not exist in the user's internal RefreshTokens list.
        RefreshSessionCommand command = RefreshSessionCommandTestBuilder.ACommand()
            .WithRefreshToken("non-existent-token")
            .Build();

        _userRepositoryMock.GetByRefreshTokenAsync(command.RefreshToken, Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        Result<SessionResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();

        // The error should be propagated from the Domain Entity
        result.Error.Should().Be(UserErrors.InvalidRefreshToken);

        _jwtServiceMock.DidNotReceiveWithAnyArgs().GenerateToken(Arg.Any<User>());

        await _userRepositoryMock.DidNotReceiveWithAnyArgs()
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessWithNewTokens_When_RotationIsSuccessful()
    {
        // Arrange
        User user = UserTestBuilder.AUser().Build();
        string expectedAccessToken = "new-jwt-access-token";

        // Create actual token instances using the internal domain method
        RefreshToken oldToken = RefreshToken.Create(user.Id, TimeSpan.FromDays(1));
        RefreshToken newToken = RefreshToken.Create(user.Id, TimeSpan.FromDays(7));

        // Configure the factory to return our tokens in sequence
        _refreshTokenFactoryMock.CreateForUser(user.Id).Returns(oldToken, newToken);

        // Add the old token to the user so it exists when rotation is attempted
        user.AddRefreshToken(_refreshTokenFactoryMock);

        RefreshSessionCommand command = RefreshSessionCommandTestBuilder.ACommand()
            .WithRefreshToken(oldToken.Token)
            .Build();

        _userRepositoryMock.GetByRefreshTokenAsync(command.RefreshToken, Arg.Any<CancellationToken>())
            .Returns(user);

        _jwtServiceMock.GenerateToken(user).Returns(expectedAccessToken);

        // Act
        Result<SessionResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be(expectedAccessToken);
        result.Value.RefreshToken.Should().Be(newToken.Token);

        // Verify side effects
        await _userRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        _jwtServiceMock.Received(1).GenerateToken(user);
    }
}
