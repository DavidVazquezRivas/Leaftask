using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Organizations.Application.Roles.GetMyPermissions;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;
using Modules.Organizations.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Organizations.Application.UnitTests.Roles.GetMyPermissions;

public class GetMyOrganizationPermissionsQueryHandlerTests
{
    private readonly GetMyOrganizationPermissionsQueryHandler _handler;
    private readonly IOrganizationRepository _repositoryMock;
    private readonly IGetMyOrganizationPermissionsQueryService _serviceMock;
    private readonly IUserContext _userContextMock;

    public GetMyOrganizationPermissionsQueryHandlerTests()
    {
        _repositoryMock = Substitute.For<IOrganizationRepository>();
        _serviceMock = Substitute.For<IGetMyOrganizationPermissionsQueryService>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new GetMyOrganizationPermissionsQueryHandler(_repositoryMock, _serviceMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnPermissions_When_UserIsMember()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Organization organization = OrganizationTestBuilder.AnOrganization().WithCreatorUserId(userId).Build();
        GetMyOrganizationPermissionsQuery query = new(organization.Id);

        _userContextMock.UserId.Returns(userId);
        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);

        IReadOnlyList<MyOrganizationPermissionDto> expected = [];
        _serviceMock.GetMyPermissionsAsync(organization.Id, userId, Arg.Any<CancellationToken>()).Returns(expected);

        // Act
        Result<IReadOnlyList<MyOrganizationPermissionDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_OrganizationNotFound()
    {
        // Arrange
        Guid orgId = Guid.NewGuid();
        _repositoryMock.GetByIdAsync(orgId, Arg.Any<CancellationToken>()).Returns((Organization?)null);
        GetMyOrganizationPermissionsQuery query = new(orgId);

        // Act
        Result<IReadOnlyList<MyOrganizationPermissionDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserIsNotMember()
    {
        // Arrange
        Guid creatorId = Guid.NewGuid();
        Guid otherId = Guid.NewGuid();
        Organization organization = OrganizationTestBuilder.AnOrganization().WithCreatorUserId(creatorId).Build();
        GetMyOrganizationPermissionsQuery query = new(organization.Id);

        _userContextMock.UserId.Returns(otherId);
        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);

        // Act
        Result<IReadOnlyList<MyOrganizationPermissionDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationMembershipRequired);
    }
}
