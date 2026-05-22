using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Users.Application.Session;
using Modules.Users.Application.Session.Jwt;
using Modules.Users.Application.Session.OAuth.Google;
using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Factories;
using Modules.Users.Domain.Repositories;
using Modules.Users.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Users.Application.UnitTests.Session.OAuth.Google;

public class LoginGoogleCommandHandlerTests
{
    private readonly IGoogleTokenValidator _googleTokenValidatorMock;
    private readonly LoginGoogleCommandHandler _handler;
    private readonly IJwtService _jwtServiceMock;
    private readonly IRefreshTokenFactory _refreshTokenFactoryMock;
    private readonly IUserRepository _userRepositoryMock;

    public LoginGoogleCommandHandlerTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _googleTokenValidatorMock = Substitute.For<IGoogleTokenValidator>();
        _jwtServiceMock = Substitute.For<IJwtService>();
        _refreshTokenFactoryMock = Substitute.For<IRefreshTokenFactory>();

        _handler = new LoginGoogleCommandHandler(
            _userRepositoryMock,
            _googleTokenValidatorMock,
            _jwtServiceMock,
            _refreshTokenFactoryMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnValidationFailure_When_GoogleTokenIsInvalid()
    {
        // Arrange
        LoginGoogleCommand command = LoginGoogleCommandTestBuilder.ACommand().Build();
        Error validationError = new("Google.InvalidToken", "The provided Google token is invalid", 400);

        _googleTokenValidatorMock.ValidateAsync(command.GoogleIdToken, Arg.Any<CancellationToken>())
            .Returns(Result.Failure<GoogleUserPayload>(validationError));

        // Act
        Result<SessionResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(validationError);

        await _userRepositoryMock.DidNotReceiveWithAnyArgs().GetByEmailAsync(default!);
        await _userRepositoryMock.DidNotReceiveWithAnyArgs().SaveChangesAsync();
    }

    [Fact]
    public async Task Handle_Should_LoginExistingUser_When_UserAlreadyExists()
    {
        // Arrange
        LoginGoogleCommand command = LoginGoogleCommandTestBuilder.ACommand().Build();
        GoogleUserPayload payload = GoogleUserPayloadTestBuilder.APayload().Build();
        User existingUser = UserTestBuilder.AUser().WithEmail(payload.Email).Build();
        RefreshToken refreshToken = RefreshToken.Create(existingUser.Id, TimeSpan.FromDays(7));
        string expectedJwt = "existing-user-jwt";

        _googleTokenValidatorMock.ValidateAsync(command.GoogleIdToken, Arg.Any<CancellationToken>())
            .Returns(Result.Success(payload));

        _userRepositoryMock.GetByEmailAsync(payload.Email, Arg.Any<CancellationToken>())
            .Returns(existingUser);

        _refreshTokenFactoryMock.CreateForUser(existingUser.Id).Returns(refreshToken);
        _jwtServiceMock.GenerateToken(existingUser).Returns(expectedJwt);

        // Act
        Result<SessionResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be(expectedJwt);
        result.Value.RefreshToken.Should().Be(refreshToken.Token);

        // Verify side effects: Should save, but MUST NOT add a new user
        await _userRepositoryMock.DidNotReceiveWithAnyArgs().AddAsync(default!);
        await _userRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        existingUser.RefreshTokens.Should().Contain(refreshToken);
    }

    [Fact]
    public async Task Handle_Should_CreateNewUserAndLogin_When_UserDoesNotExist()
    {
        // Arrange
        LoginGoogleCommand command = LoginGoogleCommandTestBuilder.ACommand().Build();
        GoogleUserPayload payload = GoogleUserPayloadTestBuilder.APayload().Build();
        string expectedJwt = "new-user-jwt";

        // Since the user is generated inside the handler, we don't know the ID in advance.
        // We use Arg.Any<Guid>() to ensure the factory returns a token regardless of the new ID.
        RefreshToken refreshToken = RefreshToken.Create(Guid.NewGuid(), TimeSpan.FromDays(7));

        _googleTokenValidatorMock.ValidateAsync(command.GoogleIdToken, Arg.Any<CancellationToken>())
            .Returns(Result.Success(payload));

        _userRepositoryMock.GetByEmailAsync(payload.Email, Arg.Any<CancellationToken>())
            .Returns((User)null!);

        _refreshTokenFactoryMock.CreateForUser(Arg.Any<Guid>()).Returns(refreshToken);

        // We also use Arg.Any<User>() because the User instance is created inside the handler.
        _jwtServiceMock.GenerateToken(Arg.Any<User>()).Returns(expectedJwt);

        // Act
        Result<SessionResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be(expectedJwt);

        // Verify side effects: MUST add the new user and save changes
        await _userRepositoryMock.Received(1).AddAsync(
            Arg.Is<User>(u => u.Email == payload.Email && u.FirstName == payload.FirstName),
            Arg.Any<CancellationToken>());

        await _userRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
