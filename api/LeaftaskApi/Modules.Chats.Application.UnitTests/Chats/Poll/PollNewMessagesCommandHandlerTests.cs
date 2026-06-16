using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Chats.Application.Chats;
using Modules.Chats.Application.Chats.Poll;
using NSubstitute;

namespace Modules.Chats.Application.UnitTests.Chats.Poll;

public class PollNewMessagesCommandHandlerTests
{
    private readonly PollNewMessagesCommandHandler _handler;
    private readonly IPollMessagesService _pollServiceMock;
    private readonly IUserContext _userContextMock;

    public PollNewMessagesCommandHandlerTests()
    {
        _pollServiceMock = Substitute.For<IPollMessagesService>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new PollNewMessagesCommandHandler(_pollServiceMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyList_When_NoNewMessages()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);
        _pollServiceMock.PollAsync(userId, Arg.Any<CancellationToken>()).Returns([]);

        // Act
        Result<List<ChatMessageDto>> result = await _handler.Handle(new PollNewMessagesCommand(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnMessages_When_NewMessagesExist()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        ChatSenderDto sender = new(Guid.NewGuid(), "Alice Doe", "person");
        List<ChatMessageDto> messages =
        [
            new(Guid.NewGuid(), Guid.NewGuid(), "Hello!", DateTime.UtcNow, "delivered", sender),
            new(Guid.NewGuid(), Guid.NewGuid(), "How are you?", DateTime.UtcNow.AddSeconds(1), "pending", null)
        ];

        _pollServiceMock.PollAsync(userId, Arg.Any<CancellationToken>()).Returns(messages);

        // Act
        Result<List<ChatMessageDto>> result = await _handler.Handle(new PollNewMessagesCommand(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value[0].Content.Should().Be("Hello!");
        result.Value[0].Status.Should().Be("delivered");
    }

    [Fact]
    public async Task Handle_Should_PassCurrentUserId_To_PollService()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);
        _pollServiceMock.PollAsync(userId, Arg.Any<CancellationToken>()).Returns([]);

        // Act
        await _handler.Handle(new PollNewMessagesCommand(), CancellationToken.None);

        // Assert
        await _pollServiceMock.Received(1).PollAsync(userId, Arg.Any<CancellationToken>());
    }
}
