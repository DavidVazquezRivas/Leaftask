using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Notification.Application.ApprovalRequests.GetMy;
using NSubstitute;

namespace Modules.Notification.Application.UnitTests.ApprovalRequests.GetMy;

public class GetMyApprovalsQueryHandlerTests
{
    private readonly GetMyApprovalsQueryHandler _handler;
    private readonly IGetMyApprovalsQueryService _serviceMock;
    private readonly IUserContext _userContextMock;

    public GetMyApprovalsQueryHandlerTests()
    {
        _serviceMock = Substitute.For<IGetMyApprovalsQueryService>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new GetMyApprovalsQueryHandler(_serviceMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnPaginatedApprovals_For_CurrentUser()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        GetMyApprovalsQuery query = new() { Limit = 5 };
        PaginatedResult<ApprovalDto> expected = new([], null, false);
        _serviceMock.GetMyApprovalsAsync(userId, 5, null, Arg.Any<CancellationToken>()).Returns(expected);

        // Act
        Result<PaginatedResult<ApprovalDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expected);
    }
}
