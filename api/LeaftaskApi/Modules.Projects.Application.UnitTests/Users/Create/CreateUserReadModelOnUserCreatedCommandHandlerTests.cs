using Modules.Projects.Application.Users.Create;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Repositories;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Users.Create;

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
    public async Task Handle_Should_AddUserReadModel_When_UserDoesNotExist()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        CreateUserReadModelOnUserCreatedCommand command = new(userId, "John", "Doe", "john@example.com");

        _repositoryMock.ExistsByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _repositoryMock.Received(1).AddAsync(
            Arg.Is<UserReadModel>(m =>
                m.Id == userId &&
                m.FirstName == "John" &&
                m.LastName == "Doe" &&
                m.Email == "john@example.com"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_NotAddUserReadModel_When_UserAlreadyExists()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        CreateUserReadModelOnUserCreatedCommand command = new(userId, "John", "Doe", "john@example.com");

        _repositoryMock.ExistsByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _repositoryMock.DidNotReceive().AddAsync(Arg.Any<UserReadModel>(), Arg.Any<CancellationToken>());
    }
}
