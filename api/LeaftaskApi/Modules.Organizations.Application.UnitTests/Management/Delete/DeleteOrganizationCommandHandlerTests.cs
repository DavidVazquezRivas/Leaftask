using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Organizations.Application.Management.Delete;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;
using Modules.Organizations.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Organizations.Application.UnitTests.Management.Delete;

public class DeleteOrganizationCommandHandlerTests
{
    private readonly DeleteOrganizationCommandHandler _handler;
    private readonly IOrganizationRepository _repositoryMock;

    public DeleteOrganizationCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<IOrganizationRepository>();
        _handler = new DeleteOrganizationCommandHandler(_repositoryMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_OrganizationExists()
    {
        // Arrange
        Organization organization = OrganizationTestBuilder.AnOrganization().Build();
        DeleteOrganizationCommand command = new(organization.Id);

        _repositoryMock.GetByIdAsync(organization.Id, Arg.Any<CancellationToken>())
            .Returns(organization);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _repositoryMock.Received(1).RemoveAsync(organization, Arg.Any<CancellationToken>());
        await _repositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_OrganizationDoesNotExist()
    {
        // Arrange
        Guid organizationId = Guid.NewGuid();
        DeleteOrganizationCommand command = new(organizationId);

        _repositoryMock.GetByIdAsync(organizationId, Arg.Any<CancellationToken>())
            .Returns((Organization?)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationNotFound);
        await _repositoryMock.DidNotReceive().RemoveAsync(Arg.Any<Organization>(), Arg.Any<CancellationToken>());
        await _repositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
