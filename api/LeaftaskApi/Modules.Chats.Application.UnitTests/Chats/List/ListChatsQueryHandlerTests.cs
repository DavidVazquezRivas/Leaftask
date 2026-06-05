using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Chats.Application.Chats;
using Modules.Chats.Application.Chats.List;
using NSubstitute;

namespace Modules.Chats.Application.UnitTests.Chats.List;

public class ListChatsQueryHandlerTests
{
    private readonly ListChatsQueryHandler _handler;
    private readonly IListChatsQueryService _queryServiceMock;
    private readonly IUserContext _userContextMock;

    public ListChatsQueryHandlerTests()
    {
        _queryServiceMock = Substitute.For<IListChatsQueryService>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new ListChatsQueryHandler(_queryServiceMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyList_When_UserHasNoChats()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);
        _queryServiceMock.ListChatsAsync(userId, Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        Result<List<ChatDto>> result = await _handler.Handle(new ListChatsQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnChats_When_UserHasChats()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        ChatLastMessageDto lastMessage = new("Hello!", DateTime.UtcNow, "delivered");
        List<ChatDto> chats =
        [
            new(Guid.NewGuid(), "Alice Doe", lastMessage, "person", null, null, 0),
            new(Guid.NewGuid(), "AI Assistant", new ChatLastMessageDto("Hi", DateTime.UtcNow.AddMinutes(-5), "read"), "agent", null, null, 0)
        ];

        _queryServiceMock.ListChatsAsync(userId, Arg.Any<CancellationToken>())
            .Returns(chats);

        // Act
        Result<List<ChatDto>> result = await _handler.Handle(new ListChatsQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value[0].Name.Should().Be("Alice Doe");
        result.Value[0].Type.Should().Be("person");
        result.Value[1].Name.Should().Be("AI Assistant");
        result.Value[1].Type.Should().Be("agent");
    }

    [Fact]
    public async Task Handle_Should_PassCurrentUserId_To_QueryService()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);
        _queryServiceMock.ListChatsAsync(userId, Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        await _handler.Handle(new ListChatsQuery(), CancellationToken.None);

        // Assert
        await _queryServiceMock.Received(1).ListChatsAsync(userId, Arg.Any<CancellationToken>());
    }
}
