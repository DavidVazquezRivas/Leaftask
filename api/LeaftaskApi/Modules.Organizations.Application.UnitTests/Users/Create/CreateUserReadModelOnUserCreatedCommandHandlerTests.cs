using FluentAssertions;
using Modules.Organizations.Application.UnitTests.TestBuilders;
using Modules.Organizations.Application.Users.Create;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Repositories;
using NSubstitute;

namespace Modules.Organizations.Application.UnitTests.Users.Create;

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
        CreateUserReadModelOnUserCreatedCommand command =
            CreateUserReadModelOnUserCreatedCommandTestBuilder.ACommand()
                .WithFirstName("Clark")
                .WithLastName("Kent")
                .WithEmail("clark@dailyplanet.com")
                .Build();

        _repositoryMock.ExistsByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _repositoryMock.Received(1)
            .AddAsync(Arg.Is<UserReadModel>(user =>
                    user.Id == command.UserId &&
                    user.FirstName == command.FirstName &&
                    user.LastName == command.LastName &&
                    user.Email == command.Email),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_NotAddUserReadModel_When_UserAlreadyExists()
    {
        // Arrange
        CreateUserReadModelOnUserCreatedCommand command =
            CreateUserReadModelOnUserCreatedCommandTestBuilder.ACommand().Build();

        _repositoryMock.ExistsByIdAsync(command.UserId, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _repositoryMock.DidNotReceive()
            .AddAsync(Arg.Any<UserReadModel>(), Arg.Any<CancellationToken>());
    }
}
