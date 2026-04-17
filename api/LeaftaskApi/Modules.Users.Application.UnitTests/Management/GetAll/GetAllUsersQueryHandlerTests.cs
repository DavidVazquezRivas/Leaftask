using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Users.Application.Management.GetAll;
using NSubstitute;

namespace Modules.Users.Application.UnitTests.Management.GetAll;

public class GetAllUsersQueryHandlerTests
{
    private readonly GetAllUsersQueryHandler _handler;
    private readonly IGetAllUsersQueryService _serviceMock;

    public GetAllUsersQueryHandlerTests()
    {
        _serviceMock = Substitute.For<IGetAllUsersQueryService>();
        _handler = new GetAllUsersQueryHandler(_serviceMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessResultWithUsers_When_ServiceReturnsData()
    {
        // Arrange
        string cursor = "cursor-token";
        GetAllUsersQuery query = GetAllUsersQueryTestBuilder.AQuery()
            .WithLimit(2)
            .WithCursor(cursor)
            .WithSort(["email:asc"])
            .Build();

        List<SimpleUserDto> userDtos =
        [
            SimpleUserDtoTestBuilder.ADto().Build(),
            SimpleUserDtoTestBuilder.ADto().Build()
        ];

        _serviceMock.GetAllAsync(query.Limit, query.Cursor, query.Sort, Arg.Any<CancellationToken>())
            .Returns(new PaginatedResult<SimpleUserDto>(userDtos, "next-cursor", true));

        // Act
        Result<PaginatedResult<SimpleUserDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(2);
        result.Value.Items.Should().BeEquivalentTo(userDtos);
        result.Value.NextCursor.Should().Be("next-cursor");
        result.Value.HasMore.Should().BeTrue();

        await _serviceMock.Received(1).GetAllAsync(query.Limit, query.Cursor, query.Sort, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyCollection_When_ServiceReturnsNoUsers()
    {
        // Arrange
        GetAllUsersQuery query = GetAllUsersQueryTestBuilder.AQuery().Build();

        _serviceMock.GetAllAsync(query.Limit, query.Cursor, query.Sort, Arg.Any<CancellationToken>())
            .Returns(new PaginatedResult<SimpleUserDto>([], null, false));

        // Act
        Result<PaginatedResult<SimpleUserDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
        result.Value.NextCursor.Should().BeNull();
        result.Value.HasMore.Should().BeFalse();
    }
}
