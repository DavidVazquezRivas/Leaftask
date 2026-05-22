using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Users.Application.Session.Logout;
using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Errors;
using Modules.Users.Domain.Repositories;
using Modules.Users.Domain.Specifications;
using Modules.Users.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Users.Application.UnitTests.Session.Logout;

public class LogoutCommandHandlerTests
{
    private readonly LogoutCommandHandler _handler;
    private readonly IUserContext _userContextMock;
    private readonly IUserRepository _userRepositoryMock;

    public LogoutCommandHandlerTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new LogoutCommandHandler(_userRepositoryMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserIsUnauthenticated()
    {
        // Arrange
        _userContextMock.UserId.Returns(Guid.Empty);
        LogoutCommand command = new();

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Unauthenticated);

        await _userRepositoryMock.DidNotReceiveWithAnyArgs()
            .GetBySpecAsync(Arg.Any<UserWithActiveRefreshTokensSpecification>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserNotFoundInRepository()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        _userRepositoryMock.GetBySpecAsync(Arg.Any<UserWithActiveRefreshTokensSpecification>(),
                Arg.Any<CancellationToken>())
            .Returns((User)null!);

        LogoutCommand command = new();

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.UserNotFound);
    }

    [Fact]
    public async Task Handle_Should_RevokeTokensAndSave_When_UserIsFound()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        User user = UserTestBuilder.AUser().Build();

        _userRepositoryMock.GetBySpecAsync(Arg.Any<UserWithActiveRefreshTokensSpecification>(),
                Arg.Any<CancellationToken>())
            .Returns(user);

        LogoutCommand command = new();

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        await _userRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        // We can verify the side effect on the domain object directly
        user.RefreshTokens.Should().OnlyContain(t => t.RevokedAt != null);
    }
}
