using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Chats.Application.Chats;
using Modules.Chats.Application.Chats.Messages.Send;
using Modules.Chats.Domain.Entities;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.Domain.Errors;
using Modules.Chats.Domain.Repositories;
using NSubstitute;

namespace Modules.Chats.Application.UnitTests.Chats.Messages.Send;

public class SendMessageCommandHandlerTests
{
    private readonly SendMessageCommandHandler _handler;
    private readonly IChatRepository _chatRepositoryMock;
    private readonly IChatMessageRepository _messageRepositoryMock;
    private readonly IUserReadModelRepository _userReadModelRepositoryMock;
    private readonly IAgentReadModelRepository _agentReadModelRepositoryMock;
    private readonly IUserContext _userContextMock;

    public SendMessageCommandHandlerTests()
    {
        _chatRepositoryMock = Substitute.For<IChatRepository>();
        _messageRepositoryMock = Substitute.For<IChatMessageRepository>();
        _userReadModelRepositoryMock = Substitute.For<IUserReadModelRepository>();
        _agentReadModelRepositoryMock = Substitute.For<IAgentReadModelRepository>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new SendMessageCommandHandler(
            _chatRepositoryMock,
            _messageRepositoryMock,
            _userReadModelRepositoryMock,
            _agentReadModelRepositoryMock,
            _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_ChatDoesNotExist()
    {
        // Arrange
        Guid chatId = Guid.NewGuid();
        _chatRepositoryMock.GetByIdTrackedAsync(chatId, Arg.Any<CancellationToken>()).Returns((Chat?)null);

        // Act
        Result<ChatMessageDto> result = await _handler.Handle(
            new SendMessageCommand(chatId, "Hello!"), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ChatErrors.ChatNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnForbidden_When_UserIsNotParticipant()
    {
        // Arrange
        Guid chatId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        Chat chat = Chat.Create(chatId, DateTime.UtcNow);

        _userContextMock.UserId.Returns(userId);
        _chatRepositoryMock.GetByIdTrackedAsync(chatId, Arg.Any<CancellationToken>()).Returns(chat);
        _chatRepositoryMock.GetParticipantTrackedAsync(chatId, userId, Arg.Any<CancellationToken>())
            .Returns((ChatParticipant?)null);

        // Act
        Result<ChatMessageDto> result = await _handler.Handle(
            new SendMessageCommand(chatId, "Hello!"), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ChatErrors.NotParticipant);
    }

    [Fact]
    public async Task Handle_Should_ReturnCreatedMessage_When_UserIsParticipant()
    {
        // Arrange
        Guid chatId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        Chat chat = Chat.Create(chatId, DateTime.UtcNow);
        ChatParticipant participant = new(Guid.NewGuid(), userId, ParticipantType.User, DateTime.UtcNow, chat);
        UserReadModel user = new(userId, "Alice", "Doe");

        _userContextMock.UserId.Returns(userId);
        _chatRepositoryMock.GetByIdTrackedAsync(chatId, Arg.Any<CancellationToken>()).Returns(chat);
        _chatRepositoryMock.GetParticipantTrackedAsync(chatId, userId, Arg.Any<CancellationToken>())
            .Returns(participant);
        _userReadModelRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        Result<ChatMessageDto> result = await _handler.Handle(
            new SendMessageCommand(chatId, "Hello!"), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ChatId.Should().Be(chatId);
        result.Value.Content.Should().Be("Hello!");
        result.Value.Status.Should().Be("pending");
        result.Value.Sender!.Name.Should().Be("Alice Doe");
        result.Value.Sender.Type.Should().Be("person");
    }
}
