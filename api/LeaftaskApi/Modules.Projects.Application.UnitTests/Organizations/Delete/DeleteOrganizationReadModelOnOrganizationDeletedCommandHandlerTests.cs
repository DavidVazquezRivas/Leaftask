using Modules.Projects.Application.Organizations.Delete;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Repositories;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Organizations.Delete;

public class DeleteOrganizationReadModelOnOrganizationDeletedCommandHandlerTests
{
    private readonly DeleteOrganizationReadModelOnOrganizationDeletedCommandHandler _handler;
    private readonly IOrganizationReadModelRepository _repositoryMock;

    public DeleteOrganizationReadModelOnOrganizationDeletedCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<IOrganizationReadModelRepository>();
        _handler = new DeleteOrganizationReadModelOnOrganizationDeletedCommandHandler(_repositoryMock);
    }

    [Fact]
    public async Task Handle_Should_RemoveOrganizationReadModel_When_OrganizationExists()
    {
        // Arrange
        Guid organizationId = Guid.NewGuid();
        DeleteOrganizationReadModelOnOrganizationDeletedCommand command = new(organizationId);

        OrganizationReadModel model = new(organizationId);
        _repositoryMock.GetByIdAsync(organizationId, Arg.Any<CancellationToken>())
            .Returns(model);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _repositoryMock.Received(1).RemoveAsync(model, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_DoNothing_When_OrganizationDoesNotExist()
    {
        // Arrange
        Guid organizationId = Guid.NewGuid();
        DeleteOrganizationReadModelOnOrganizationDeletedCommand command = new(organizationId);

        _repositoryMock.GetByIdAsync(organizationId, Arg.Any<CancellationToken>())
            .Returns((OrganizationReadModel?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _repositoryMock.DidNotReceive().RemoveAsync(Arg.Any<OrganizationReadModel>(), Arg.Any<CancellationToken>());
    }
}
