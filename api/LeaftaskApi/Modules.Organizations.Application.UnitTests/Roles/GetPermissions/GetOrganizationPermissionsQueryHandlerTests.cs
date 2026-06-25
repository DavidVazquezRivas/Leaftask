using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Organizations.Application.Roles.GetPermissions;
using NSubstitute;

namespace Modules.Organizations.Application.UnitTests.Roles.GetPermissions;

public class GetOrganizationPermissionsQueryHandlerTests
{
    private readonly GetOrganizationPermissionsQueryHandler _handler;
    private readonly IGetOrganizationPermissionsQueryService _serviceMock;

    public GetOrganizationPermissionsQueryHandlerTests()
    {
        _serviceMock = Substitute.For<IGetOrganizationPermissionsQueryService>();
        _handler = new GetOrganizationPermissionsQueryHandler(_serviceMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnAllPermissions()
    {
        // Arrange
        IReadOnlyList<OrganizationPermissionDto> expected = [new(Guid.NewGuid(), "org.read", "Read org")];
        _serviceMock.GetPermissionsAsync(Arg.Any<CancellationToken>()).Returns(expected);

        // Act
        Result<IReadOnlyList<OrganizationPermissionDto>> result =
            await _handler.Handle(new GetOrganizationPermissionsQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(expected);
    }
}
