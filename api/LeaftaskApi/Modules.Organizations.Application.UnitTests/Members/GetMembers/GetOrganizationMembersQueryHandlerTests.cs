using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Queries;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Organizations.Application.Members.GetMembers;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;
using Modules.Organizations.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Organizations.Application.UnitTests.Members.GetMembers;

public class GetOrganizationMembersQueryHandlerTests
{
    private readonly GetOrganizationMembersQueryHandler _handler;
    private readonly IGetOrganizationMembersQueryService _serviceMock;
    private readonly IOrganizationRepository _repositoryMock;
    private readonly IUserContext _userContextMock;

    public GetOrganizationMembersQueryHandlerTests()
    {
        _repositoryMock = Substitute.For<IOrganizationRepository>();
        _serviceMock = Substitute.For<IGetOrganizationMembersQueryService>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new GetOrganizationMembersQueryHandler(_repositoryMock, _serviceMock, _userContextMock);
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

        GetOrganizationMembersQuery query = new()
        {
            OrganizationId = organization.Id,
            Limit = 10,
            Sort = []
        };

        PaginatedResult<OrganizationMemberDto> paginatedResult = new(
            [new OrganizationMemberDto(Guid.NewGuid(), "John Doe", "john.doe@example.com", Guid.NewGuid())],
            null,
            false);

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);
        _serviceMock.GetOrganizationMembersAsync(organization.Id, query.Limit, query.Cursor, query.Sort, Arg.Any<CancellationToken>())
            .Returns(paginatedResult);

        // Act
        Result<PaginatedResult<OrganizationMemberDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(paginatedResult);
        await _serviceMock.Received(1).GetOrganizationMembersAsync(organization.Id, query.Limit, query.Cursor, query.Sort, Arg.Any<CancellationToken>());
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

        GetOrganizationMembersQuery query = new()
        {
            OrganizationId = organization.Id,
            Limit = 10,
            Sort = []
        };

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>()).Returns(organization);

        // Act
        Result<PaginatedResult<OrganizationMemberDto>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationMembershipRequired);
        await _serviceMock.DidNotReceive().GetOrganizationMembersAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<string?>(), Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<CancellationToken>());
    }
}
