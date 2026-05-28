using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Chats.Application.Chats;
using Modules.Chats.Application.Chats.Create;
using Modules.Chats.Domain.Entities;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.Domain.Errors;
using Modules.Chats.Domain.Repositories;
using NSubstitute;

namespace Modules.Chats.Application.UnitTests.Chats.Create;

public class CreateChatCommandHandlerTests
{
    private readonly CreateChatCommandHandler _handler;
    private readonly IChatRepository _chatRepositoryMock;
    private readonly IUserReadModelRepository _userReadModelRepositoryMock;
    private readonly IUserContext _userContextMock;

    public CreateChatCommandHandlerTests()
    {
        _chatRepositoryMock = Substitute.For<IChatRepository>();
        _userReadModelRepositoryMock = Substitute.For<IUserReadModelRepository>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new CreateChatCommandHandler(
            _chatRepositoryMock,
            _userReadModelRepositoryMock,
            _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnParticipantNotFound_When_TargetUserDoesNotExist()
    {
        // Arrange
        Guid otherId = Guid.NewGuid();
        _userContextMock.UserId.Returns(Guid.NewGuid());
        _userReadModelRepositoryMock.ExistsByIdAsync(otherId, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result<ChatDto> result = await _handler.Handle(
            new CreateChatCommand(otherId, "person"), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ChatErrors.ParticipantNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnAlreadyExists_When_ChatAlreadyExists()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid otherId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);
        _userReadModelRepositoryMock.ExistsByIdAsync(otherId, Arg.Any<CancellationToken>()).Returns(true);
        _chatRepositoryMock
            .ExistsBetweenParticipantsAsync(userId, otherId, ParticipantType.User, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        Result<ChatDto> result = await _handler.Handle(
            new CreateChatCommand(otherId, "person"), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ChatErrors.AlreadyExists);
    }

    [Fact]
    public async Task Handle_Should_CreateChatWithPersonType_When_OtherParticipantIsUser()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid otherId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);
        _userReadModelRepositoryMock.ExistsByIdAsync(otherId, Arg.Any<CancellationToken>()).Returns(true);
        _chatRepositoryMock
            .ExistsBetweenParticipantsAsync(userId, otherId, ParticipantType.User, Arg.Any<CancellationToken>())
            .Returns(false);
        _userReadModelRepositoryMock.GetByIdAsync(otherId, Arg.Any<CancellationToken>())
            .Returns(new UserReadModel(otherId, "Bob", "Smith"));

        // Act
        Result<ChatDto> result = await _handler.Handle(
            new CreateChatCommand(otherId, "person"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Bob Smith");
        result.Value.Type.Should().Be("person");
        result.Value.LastMessage.Should().BeNull();
    }

    [Fact]
    public async Task Handle_Should_CreateChatWithAgentType_When_OtherParticipantIsAgent()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid agentId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);
        _chatRepositoryMock
            .ExistsBetweenParticipantsAsync(userId, agentId, ParticipantType.Agent, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        Result<ChatDto> result = await _handler.Handle(
            new CreateChatCommand(agentId, "agent"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("AI Assistant");
        result.Value.Type.Should().Be("agent");
    }
}
