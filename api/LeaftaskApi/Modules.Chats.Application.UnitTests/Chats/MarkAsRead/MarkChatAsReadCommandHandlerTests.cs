using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Chats.Application.Chats.MarkAsRead;
using Modules.Chats.Domain.Entities;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.Domain.Errors;
using Modules.Chats.Domain.Repositories;
using NSubstitute;

namespace Modules.Chats.Application.UnitTests.Chats.MarkAsRead;

public class MarkChatAsReadCommandHandlerTests
{
    private readonly MarkChatAsReadCommandHandler _handler;
    private readonly IChatRepository _chatRepositoryMock;
    private readonly IChatMessageRepository _chatMessageRepositoryMock;
    private readonly IUserContext _userContextMock;

    public MarkChatAsReadCommandHandlerTests()
    {
        _chatRepositoryMock = Substitute.For<IChatRepository>();
        _chatMessageRepositoryMock = Substitute.For<IChatMessageRepository>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new MarkChatAsReadCommandHandler(_chatRepositoryMock, _chatMessageRepositoryMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ParticipantExists()
    {
        // Arrange
        Guid chatId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        Chat chat = Chat.Create(chatId, DateTime.UtcNow);
        ChatParticipant participant = new(Guid.NewGuid(), userId, ParticipantType.User, DateTime.UtcNow, chat);

        _userContextMock.UserId.Returns(userId);
        _chatRepositoryMock.GetParticipantAsync(chatId, userId, Arg.Any<CancellationToken>()).Returns(participant);

        MarkChatAsReadCommand command = new(chatId);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _chatMessageRepositoryMock.Received(1)
            .MarkChatMessagesAsReadAsync(chatId, userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NotParticipant()
    {
        // Arrange
        Guid chatId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);
        _chatRepositoryMock.GetParticipantAsync(chatId, userId, Arg.Any<CancellationToken>()).Returns((ChatParticipant?)null);

        MarkChatAsReadCommand command = new(chatId);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ChatErrors.NotParticipant);
    }
}
