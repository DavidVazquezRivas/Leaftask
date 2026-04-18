using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Organizations.Application.Roles.GetRoles;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;
using Modules.Organizations.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Organizations.Application.UnitTests.Roles.GetRoles;

public class GetOrganizationRolesQueryHandlerTests
{
    private readonly GetOrganizationRolesQueryHandler _handler;
    private readonly IGetOrganizationRolesQueryService _serviceMock;
    private readonly IOrganizationRepository _repositoryMock;
    private readonly IUserContext _userContextMock;

    public GetOrganizationRolesQueryHandlerTests()
    {
        _repositoryMock = Substitute.For<IOrganizationRepository>();
        _serviceMock = Substitute.For<IGetOrganizationRolesQueryService>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new GetOrganizationRolesQueryHandler(_repositoryMock, _serviceMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_UserIsMember()
    {
        // Arrange
        Guid memberUserId = Guid.NewGuid();
        _userContextMock.UserId.Returns(memberUserId);

        Organization organization = OrganizationTestBuilder.AnOrganization()
            .WithCreatorUserId(memberUserId)
            .Build();

        GetOrganizationRolesQuery query = new(organization.Id);

        List<OrganizationRoleDto> roles =
        [
            new(Guid.NewGuid(), "Leaftask Admin", 1, [new(Guid.NewGuid(), (int)PermissionLevel.Full)])
        ];

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _serviceMock.GetOrganizationRolesAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(roles);

        // Act
        Result<IReadOnlyList<OrganizationRoleDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(roles);
        await _serviceMock.Received(1).GetOrganizationRolesAsync(organization.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserIsNotMember()
    {
        // Arrange
        Guid memberUserId = Guid.NewGuid();
        _userContextMock.UserId.Returns(Guid.NewGuid());

        Organization organization = OrganizationTestBuilder.AnOrganization()
            .WithCreatorUserId(memberUserId)
            .Build();

        GetOrganizationRolesQuery query = new(organization.Id);

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);

        // Act
        Result<IReadOnlyList<OrganizationRoleDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationMembershipRequired);
        await _serviceMock.DidNotReceive().GetOrganizationRolesAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
