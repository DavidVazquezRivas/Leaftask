using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Organizations.Application.Management;
using Modules.Organizations.Application.Management.Patch;
using Modules.Organizations.Application.UnitTests.TestBuilders;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;
using Modules.Organizations.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Organizations.Application.UnitTests.Management.Patch;

public class PatchOrganizationCommandHandlerTests
{
    private readonly PatchOrganizationCommandHandler _handler;
    private readonly IGetOrganizationDetailsQueryService _queryServiceMock;
    private readonly IOrganizationRepository _repositoryMock;

    public PatchOrganizationCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<IOrganizationRepository>();
        _queryServiceMock = Substitute.For<IGetOrganizationDetailsQueryService>();
        _handler = new PatchOrganizationCommandHandler(_repositoryMock, _queryServiceMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_OrganizationIsPatched()
    {
        // Arrange
        Organization organization = OrganizationTestBuilder.AnOrganization().Build();
        PatchOrganizationCommand command = new(organization.Id, "Updated name", null, "https://updated.example.com");

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>())
            .Returns(organization);

        _queryServiceMock.GetOrganizationDetailsAsync(organization.Id, Arg.Any<CancellationToken>())
            .Returns(new BasicOrganizationResponse(organization.Id, "Updated name", organization.Description, 0, 0, 0,
                organization.CreatedAt));

        // Act
        Result<BasicOrganizationResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Updated name");

        await _repositoryMock.Received(1).GetByIdAsync(organization.Id, Arg.Any<CancellationToken>());
        await _repositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _queryServiceMock.Received(1).GetOrganizationDetailsAsync(organization.Id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_OrganizationDoesNotExist()
    {
        // Arrange
        Guid organizationId = Guid.NewGuid();
        PatchOrganizationCommand command = new(organizationId, "Updated name", null, null);

        _repositoryMock.GetByIdAsync(organizationId, Arg.Any<CancellationToken>())
            .Returns((Organization?)null);

        // Act
        Result<BasicOrganizationResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationNotFound);

        await _repositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
