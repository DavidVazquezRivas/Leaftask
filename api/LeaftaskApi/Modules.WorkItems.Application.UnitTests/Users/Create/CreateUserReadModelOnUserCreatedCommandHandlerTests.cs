using FluentAssertions;
using Modules.WorkItems.Application.Users.Create;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.Users.Create;

public class CreateUserReadModelOnUserCreatedCommandHandlerTests
{
    private readonly CreateUserReadModelOnUserCreatedCommandHandler _handler;
    private readonly IUserReadModelRepository _userReadModelRepositoryMock;
    private readonly IWorkItemRepository _workItemRepositoryMock;

    public CreateUserReadModelOnUserCreatedCommandHandlerTests()
    {
        _userReadModelRepositoryMock = Substitute.For<IUserReadModelRepository>();
        _workItemRepositoryMock = Substitute.For<IWorkItemRepository>();

        _handler = new CreateUserReadModelOnUserCreatedCommandHandler(
            _userReadModelRepositoryMock,
            _workItemRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_CreateUserReadModel_When_NotExists()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        CreateUserReadModelOnUserCreatedCommand command = new(userId, "John", "Doe");

        _userReadModelRepositoryMock.ExistsByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _userReadModelRepositoryMock.Received(1)
            .AddAsync(Arg.Is<UserReadModel>(u => u.Id == userId && u.FirstName == "John" && u.LastName == "Doe"),
                Arg.Any<CancellationToken>());
        await _workItemRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_Skip_When_UserReadModelAlreadyExists()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        CreateUserReadModelOnUserCreatedCommand command = new(userId, "John", "Doe");

        _userReadModelRepositoryMock.ExistsByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _userReadModelRepositoryMock.DidNotReceive()
            .AddAsync(Arg.Any<UserReadModel>(), Arg.Any<CancellationToken>());
        await _workItemRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
