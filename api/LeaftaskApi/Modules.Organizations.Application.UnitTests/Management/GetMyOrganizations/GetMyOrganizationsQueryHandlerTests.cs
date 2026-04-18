using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Organizations.Application.Management.GetMyOrganizations;
using Modules.Organizations.Application.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Organizations.Application.UnitTests.Management.GetMyOrganizations;

public class GetMyOrganizationsQueryHandlerTests
{
    private readonly GetMyOrganizationsQueryHandler _handler;
    private readonly IGetMyOrganizationsQueryService _serviceMock;
    private readonly IUserContext _userContextMock;

    public GetMyOrganizationsQueryHandlerTests()
    {
        _serviceMock = Substitute.For<IGetMyOrganizationsQueryService>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new GetMyOrganizationsQueryHandler(_serviceMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccessWithOrganizations_When_UserHasOrganizations()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        GetMyOrganizationsQuery query = GetMyOrganizationsQueryTestBuilder.AQuery()
            .WithLimit(2)
            .WithCursor("cursor")
            .WithSort(["name:asc"])
            .Build();

        IReadOnlyList<SimpleOrganizationDto> organizations =
        [
            SimpleOrganizationDtoTestBuilder.ADto().WithName("Justice League").Build(),
            SimpleOrganizationDtoTestBuilder.ADto().WithName("Wayne Enterprises").Build()
        ];

        _serviceMock.GetMyOrganizationsAsync(userId, query.Limit, query.Cursor, query.Sort, Arg.Any<CancellationToken>())
            .Returns(new PaginatedResult<SimpleOrganizationDto>(organizations, organizations[^1].Name, true));

        // Act
        Result<PaginatedResult<SimpleOrganizationDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(2);
        result.Value.Items.Should().BeEquivalentTo(organizations);
        result.Value.NextCursor.Should().Be(organizations[^1].Name);
        result.Value.HasMore.Should().BeTrue();

        await _serviceMock.Received(1)
            .GetMyOrganizationsAsync(userId, query.Limit, query.Cursor, query.Sort, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyCollection_When_UserHasNoOrganizations()
    {
        // Arrange
        _userContextMock.UserId.Returns(Guid.NewGuid());

        GetMyOrganizationsQuery query = GetMyOrganizationsQueryTestBuilder.AQuery().Build();

        _serviceMock.GetMyOrganizationsAsync(Arg.Any<Guid>(), query.Limit, query.Cursor, query.Sort, Arg.Any<CancellationToken>())
            .Returns(new PaginatedResult<SimpleOrganizationDto>([], null, false));

        // Act
        Result<PaginatedResult<SimpleOrganizationDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();
        result.Value.NextCursor.Should().BeNull();
        result.Value.HasMore.Should().BeFalse();
    }
}
