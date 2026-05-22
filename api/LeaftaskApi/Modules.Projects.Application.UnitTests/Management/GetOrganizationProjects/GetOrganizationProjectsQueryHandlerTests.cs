using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Management.GetMyProjects;
using Modules.Projects.Application.Management.GetOrganizationProjects;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Management.GetOrganizationProjects;

public class GetOrganizationProjectsQueryHandlerTests
{
    private readonly GetOrganizationProjectsQueryHandler _handler;
    private readonly IGetOrganizationProjectQueryService _queryServiceMock;

    public GetOrganizationProjectsQueryHandlerTests()
    {
        _queryServiceMock = Substitute.For<IGetOrganizationProjectQueryService>();
        _handler = new GetOrganizationProjectsQueryHandler(_queryServiceMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_With_ProjectsFromService()
    {
        // Arrange
        Guid organizationId = Guid.NewGuid();

        SimpleProjectDto[] projects =
        [
            new(Guid.NewGuid(), "Org Project A", "OPA"),
            new(Guid.NewGuid(), "Org Project B", "OPB")
        ];
        PaginatedResult<SimpleProjectDto> paginatedResult = new(projects, "next-cursor", true);

        GetOrganizationProjectsQuery query = new() { OrganizationId = organizationId, Limit = 10, Cursor = null, Sort = [] };

        _queryServiceMock.GetOrganizationProjectsAsync(organizationId, query.Limit, query.Cursor, query.Sort, Arg.Any<CancellationToken>())
            .Returns(paginatedResult);

        // Act
        Result<PaginatedResult<SimpleProjectDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(2);
        result.Value.NextCursor.Should().Be("next-cursor");
        result.Value.HasMore.Should().BeTrue();
    }
}
