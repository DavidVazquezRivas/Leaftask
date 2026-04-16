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
        GetAllUsersQuery query = GetAllUsersQueryTestBuilder.AQuery().Build();

        List<SimpleUserDto> userDtos =
        [
            SimpleUserDtoTestBuilder.ADto().WithFirstName("Clark").Build(),
            SimpleUserDtoTestBuilder.ADto().WithFirstName("Diana").Build()
        ];

        _serviceMock.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(userDtos);

        // Act
        Result<IReadOnlyCollection<SimpleUserDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value.Should().BeEquivalentTo(userDtos);

        await _serviceMock.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyCollection_When_ServiceReturnsNoUsers()
    {
        // Arrange
        GetAllUsersQuery query = GetAllUsersQueryTestBuilder.AQuery().Build();

        _serviceMock.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        Result<IReadOnlyCollection<SimpleUserDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
