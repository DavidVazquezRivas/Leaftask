using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Notification.Application.Notifications.GetMy;
using NSubstitute;

namespace Modules.Notification.Application.UnitTests.Notifications.GetMy;

public class GetMyNotificationsQueryHandlerTests
{
    private readonly GetMyNotificationsQueryHandler _handler;
    private readonly IGetMyNotificationsQueryService _serviceMock;
    private readonly IUserContext _userContextMock;

    public GetMyNotificationsQueryHandlerTests()
    {
        _serviceMock = Substitute.For<IGetMyNotificationsQueryService>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new GetMyNotificationsQueryHandler(_serviceMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnPaginatedNotifications_For_CurrentUser()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        GetMyNotificationsQuery query = new() { Limit = 10, Status = "unread" };
        PaginatedResult<NotificationDto> expected = new([], null, false);

        _serviceMock.GetMyNotificationsAsync(userId, 10, null, Arg.Any<IReadOnlyCollection<string>>(), "unread", Arg.Any<CancellationToken>())
            .Returns(expected);

        // Act
        Result<PaginatedResult<NotificationDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expected);
    }
}
