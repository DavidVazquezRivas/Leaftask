using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Chats.Application.Chats.Delete;
using Modules.Chats.Domain.Entities;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.Domain.Errors;
using Modules.Chats.Domain.Repositories;
using NSubstitute;

namespace Modules.Chats.Application.UnitTests.Chats.Delete;

public class DeleteChatCommandHandlerTests
{
    private readonly DeleteChatCommandHandler _handler;
    private readonly IChatRepository _chatRepositoryMock;
    private readonly IUserContext _userContextMock;

    public DeleteChatCommandHandlerTests()
    {
        _chatRepositoryMock = Substitute.For<IChatRepository>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new DeleteChatCommandHandler(_chatRepositoryMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_ChatDoesNotExist()
    {
        // Arrange
        Guid chatId = Guid.NewGuid();
        _chatRepositoryMock.GetByIdTrackedAsync(chatId, Arg.Any<CancellationToken>()).Returns((Chat?)null);

        // Act
        Result result = await _handler.Handle(new DeleteChatCommand(chatId), CancellationToken.None);

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
        _chatRepositoryMock.GetParticipantAsync(chatId, userId, Arg.Any<CancellationToken>())
            .Returns((ChatParticipant?)null);

        // Act
        Result result = await _handler.Handle(new DeleteChatCommand(chatId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ChatErrors.NotParticipant);
    }

    [Fact]
    public async Task Handle_Should_DeleteChat_When_UserIsParticipant()
    {
        // Arrange
        Guid chatId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        Chat chat = Chat.Create(chatId, DateTime.UtcNow);
        ChatParticipant participant = new(Guid.NewGuid(), userId, ParticipantType.User, DateTime.UtcNow, chat);

        _userContextMock.UserId.Returns(userId);
        _chatRepositoryMock.GetByIdTrackedAsync(chatId, Arg.Any<CancellationToken>()).Returns(chat);
        _chatRepositoryMock.GetParticipantAsync(chatId, userId, Arg.Any<CancellationToken>())
            .Returns(participant);

        // Act
        Result result = await _handler.Handle(new DeleteChatCommand(chatId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _chatRepositoryMock.Received(1).Remove(chat);
        await _chatRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
