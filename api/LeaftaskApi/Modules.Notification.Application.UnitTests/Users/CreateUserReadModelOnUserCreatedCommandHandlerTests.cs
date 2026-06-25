using FluentAssertions;
using Modules.Notification.Application.Users.Create;
using Modules.Notification.Domain.Entities;
using Modules.Notification.Domain.Repositories;
using NSubstitute;

namespace Modules.Notification.Application.UnitTests.Users;

public class CreateUserReadModelOnUserCreatedCommandHandlerTests
{
    private readonly CreateUserReadModelOnUserCreatedCommandHandler _handler;
    private readonly IUserReadModelRepository _repositoryMock;

    public CreateUserReadModelOnUserCreatedCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<IUserReadModelRepository>();
        _handler = new CreateUserReadModelOnUserCreatedCommandHandler(_repositoryMock);
    }

    [Fact]
    public async Task Handle_Should_CreateUserReadModel_When_UserDoesNotExist()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        CreateUserReadModelOnUserCreatedCommand command = new(userId, "John", "Doe");
        _repositoryMock.ExistsByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(false);

        UserReadModel? added = null;
        await _repositoryMock.AddAsync(Arg.Do<UserReadModel>(u => added = u), Arg.Any<CancellationToken>());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        added.Should().NotBeNull();
        added!.Id.Should().Be(userId);
        added.FirstName.Should().Be("John");
        added.LastName.Should().Be("Doe");
        await _repositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_NotCreate_When_UserAlreadyExists()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        CreateUserReadModelOnUserCreatedCommand command = new(userId, "John", "Doe");
        _repositoryMock.ExistsByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _repositoryMock.DidNotReceive().AddAsync(Arg.Any<UserReadModel>(), Arg.Any<CancellationToken>());
        await _repositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
