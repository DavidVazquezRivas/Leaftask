using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Organizations.Application.Invitations;
using Modules.Organizations.Application.Invitations.GetPending;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;
using Modules.Organizations.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Organizations.Application.UnitTests.Invitations.GetPending;

public class GetPendingOrganizationInvitationsQueryHandlerTests
{
    private readonly GetPendingOrganizationInvitationsQueryHandler _handler;
    private readonly IOrganizationRepository _repositoryMock;
    private readonly IGetPendingOrganizationInvitationsQueryService _serviceMock;
    private readonly IUserContext _userContextMock;

    public GetPendingOrganizationInvitationsQueryHandlerTests()
    {
        _repositoryMock = Substitute.For<IOrganizationRepository>();
        _serviceMock = Substitute.For<IGetPendingOrganizationInvitationsQueryService>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new GetPendingOrganizationInvitationsQueryHandler(_repositoryMock, _serviceMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnPendingInvitations_When_UserIsMember()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Organization organization = OrganizationTestBuilder.AnOrganization().WithCreatorUserId(userId).Build();
        GetPendingOrganizationInvitationsQuery query = new(organization.Id);

        _userContextMock.UserId.Returns(userId);
        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);

        IReadOnlyList<OrganizationInvitationResponse> expected = [];
        _serviceMock.GetPendingInvitationsAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(expected);

        // Act
        Result<IReadOnlyList<OrganizationInvitationResponse>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_OrganizationNotFound()
    {
        // Arrange
        Guid orgId = Guid.NewGuid();
        GetPendingOrganizationInvitationsQuery query = new(orgId);
        _repositoryMock.GetByIdAsync(orgId, Arg.Any<CancellationToken>()).Returns((Organization?)null);

        // Act
        Result<IReadOnlyList<OrganizationInvitationResponse>> result = await _handler.Handle(query, CancellationToken.None);

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
        GetPendingOrganizationInvitationsQuery query = new(organization.Id);

        _userContextMock.UserId.Returns(otherId);
        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);

        // Act
        Result<IReadOnlyList<OrganizationInvitationResponse>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationMembershipRequired);
    }
}
