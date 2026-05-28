using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Chats.Application.Chats;
using Modules.Chats.Application.Chats.Messages.List;
using Modules.Chats.Domain.Entities;
using Modules.Chats.Domain.Entities.Participant;
using Modules.Chats.Domain.Errors;
using Modules.Chats.Domain.Repositories;
using NSubstitute;

namespace Modules.Chats.Application.UnitTests.Chats.Messages.List;

public class ListChatMessagesQueryHandlerTests
{
    private readonly ListChatMessagesQueryHandler _handler;
    private readonly IListChatMessagesQueryService _queryServiceMock;
    private readonly IChatRepository _chatRepositoryMock;
    private readonly IUserContext _userContextMock;

    public ListChatMessagesQueryHandlerTests()
    {
        _queryServiceMock = Substitute.For<IListChatMessagesQueryService>();
        _chatRepositoryMock = Substitute.For<IChatRepository>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new ListChatMessagesQueryHandler(_queryServiceMock, _chatRepositoryMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_ChatDoesNotExist()
    {
        // Arrange
        Guid chatId = Guid.NewGuid();
        ListChatMessagesQuery query = new(chatId, 10, null, []);

        _chatRepositoryMock.GetByIdAsync(chatId, Arg.Any<CancellationToken>()).Returns((Chat?)null);

        // Act
        Result<PaginatedResult<ChatMessageDto>> result = await _handler.Handle(query, CancellationToken.None);

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
        ListChatMessagesQuery query = new(chatId, 10, null, []);

        _userContextMock.UserId.Returns(userId);
        _chatRepositoryMock.GetByIdAsync(chatId, Arg.Any<CancellationToken>())
            .Returns(Chat.Create(chatId, DateTime.UtcNow));
        _chatRepositoryMock.GetParticipantAsync(chatId, userId, Arg.Any<CancellationToken>())
            .Returns((ChatParticipant?)null);

        // Act
        Result<PaginatedResult<ChatMessageDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ChatErrors.NotParticipant);
    }

    [Fact]
    public async Task Handle_Should_ReturnMessages_When_UserIsParticipant()
    {
        // Arrange
        Guid chatId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        ListChatMessagesQuery query = new(chatId, 10, null, []);

        Chat chat = Chat.Create(chatId, DateTime.UtcNow);
        ChatParticipant participant = new(Guid.NewGuid(), userId, ParticipantType.User, DateTime.UtcNow, chat);

        _userContextMock.UserId.Returns(userId);
        _chatRepositoryMock.GetByIdAsync(chatId, Arg.Any<CancellationToken>()).Returns(chat);
        _chatRepositoryMock.GetParticipantAsync(chatId, userId, Arg.Any<CancellationToken>())
            .Returns(participant);

        ChatSenderDto sender = new(userId, "Alice Doe", "person");
        PaginatedResult<ChatMessageDto> paginatedResult = new(
            [new(Guid.NewGuid(), chatId, "Hello!", DateTime.UtcNow, "read", sender)],
            null, false);

        _queryServiceMock
            .GetMessagesAsync(chatId, userId, 10, null, Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>())
            .Returns(paginatedResult);

        // Act
        Result<PaginatedResult<ChatMessageDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(1);
        result.Value.Items.First().Content.Should().Be("Hello!");
    }
}
