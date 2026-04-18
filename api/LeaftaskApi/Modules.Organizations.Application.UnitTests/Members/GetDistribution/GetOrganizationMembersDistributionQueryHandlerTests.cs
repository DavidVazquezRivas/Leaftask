using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Organizations.Application.Members.GetDistribution;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;
using Modules.Organizations.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Organizations.Application.UnitTests.Members.GetDistribution;

public class GetOrganizationMembersDistributionQueryHandlerTests
{
    private readonly GetOrganizationMembersDistributionQueryHandler _handler;
    private readonly IGetOrganizationMembersDistributionQueryService _serviceMock;
    private readonly IOrganizationRepository _repositoryMock;
    private readonly IUserContext _userContextMock;

    public GetOrganizationMembersDistributionQueryHandlerTests()
    {
        _repositoryMock = Substitute.For<IOrganizationRepository>();
        _serviceMock = Substitute.For<IGetOrganizationMembersDistributionQueryService>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new GetOrganizationMembersDistributionQueryHandler(_repositoryMock, _serviceMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_UserIsMember()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        Organization organization = OrganizationTestBuilder.AnOrganization()
            .WithCreatorUserId(userId)
            .Build();

        GetOrganizationMembersDistributionQuery query = new(organization.Id);
        List<OrganizationMemberDistributionDto> distribution =
        [
            new(Guid.NewGuid(), 5)
        ];

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _serviceMock.GetOrganizationMembersDistributionAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(distribution);

        // Act
        Result<IReadOnlyList<OrganizationMemberDistributionDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(distribution);
        await _serviceMock.Received(1).GetOrganizationMembersDistributionAsync(organization.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserIsNotMember()
    {
        // Arrange
        Guid creatorUserId = Guid.NewGuid();
        _userContextMock.UserId.Returns(Guid.NewGuid());

        Organization organization = OrganizationTestBuilder.AnOrganization()
            .WithCreatorUserId(creatorUserId)
            .Build();

        GetOrganizationMembersDistributionQuery query = new(organization.Id);

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);

        // Act
        Result<IReadOnlyList<OrganizationMemberDistributionDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationMembershipRequired);
        await _serviceMock.DidNotReceive().GetOrganizationMembersDistributionAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
