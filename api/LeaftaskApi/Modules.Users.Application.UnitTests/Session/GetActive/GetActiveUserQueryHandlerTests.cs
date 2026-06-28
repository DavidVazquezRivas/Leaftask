using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Users.Application.Session.GetActive;
using Modules.Users.Domain.Entities;
using Modules.Users.Domain.Errors;
using Modules.Users.Domain.Repositories;
using Modules.Users.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Users.Application.UnitTests.Session.GetActive;

public class GetActiveUserQueryHandlerTests
{
    private readonly GetActiveUserQueryHandler _handler;
    private readonly IUserRepository _userRepositoryMock;
    private readonly IUserContext _userContextMock;

    public GetActiveUserQueryHandlerTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new GetActiveUserQueryHandler(_userRepositoryMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnActiveUser_When_UserIsAuthenticated()
    {
        // Arrange
        User user = UserTestBuilder.AUser().Build();
        _userContextMock.UserId.Returns(user.Id);
        _userRepositoryMock.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        Result<ActiveUserResponse> result = await _handler.Handle(new GetActiveUserQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(user.Id);
        result.Value.FirstName.Should().Be(user.FirstName);
        result.Value.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserIsUnauthenticated()
    {
        // Arrange
        _userContextMock.UserId.Returns(Guid.Empty);

        // Act
        Result<ActiveUserResponse> result = await _handler.Handle(new GetActiveUserQuery(), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Unauthenticated);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserNotFound()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);
        _userRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        Result<ActiveUserResponse> result = await _handler.Handle(new GetActiveUserQuery(), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.UserNotFound);
    }
}
