using Modules.Projects.Application.Organizations.Create;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Repositories;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Organizations.Create;

public class CreateOrganizationReadModelOnOrganizationCreatedCommandHandlerTests
{
    private readonly CreateOrganizationReadModelOnOrganizationCreatedCommandHandler _handler;
    private readonly IOrganizationReadModelRepository _repositoryMock;

    public CreateOrganizationReadModelOnOrganizationCreatedCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<IOrganizationReadModelRepository>();
        _handler = new CreateOrganizationReadModelOnOrganizationCreatedCommandHandler(_repositoryMock);
    }

    [Fact]
    public async Task Handle_Should_AddOrganizationReadModel_When_OrganizationDoesNotExist()
    {
        // Arrange
        Guid organizationId = Guid.NewGuid();
        CreateOrganizationReadModelOnOrganizationCreatedCommand command = new(organizationId);

        _repositoryMock.ExistsByIdAsync(organizationId, Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _repositoryMock.Received(1).AddAsync(
            Arg.Is<OrganizationReadModel>(m => m.Id == organizationId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_NotAddOrganizationReadModel_When_OrganizationAlreadyExists()
    {
        // Arrange
        Guid organizationId = Guid.NewGuid();
        CreateOrganizationReadModelOnOrganizationCreatedCommand command = new(organizationId);

        _repositoryMock.ExistsByIdAsync(organizationId, Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _repositoryMock.DidNotReceive().AddAsync(Arg.Any<OrganizationReadModel>(), Arg.Any<CancellationToken>());
    }
}
