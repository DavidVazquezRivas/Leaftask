using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Organizations.Application.Management;
using Modules.Organizations.Application.Management.Create;
using Modules.Organizations.Application.UnitTests.TestBuilders;
using Modules.Organizations.Domain.Entities;
using Modules.Organizations.Domain.Errors;
using Modules.Organizations.Domain.Repositories;
using NSubstitute;

namespace Modules.Organizations.Application.UnitTests.Management.Create;

public class CreateOrganizationCommandHandlerTests
{
    private readonly CreateOrganizationCommandHandler _handler;
    private readonly IGetOrganizationDetailsQueryService _queryServiceMock;
    private readonly IOrganizationRepository _repositoryMock;
    private readonly IUserContext _userContextMock;

    public CreateOrganizationCommandHandlerTests()
    {
        _repositoryMock = Substitute.For<IOrganizationRepository>();
        _queryServiceMock = Substitute.For<IGetOrganizationDetailsQueryService>();
        _userContextMock = Substitute.For<IUserContext>();
        _handler = new CreateOrganizationCommandHandler(_repositoryMock, _queryServiceMock, _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_OrganizationIsCreatedAndProjected()
    {
        // Arrange
        CreateOrganizationCommand command = CreateOrganizationCommandTestBuilder.ACommand()
            .WithName("Justice League")
            .WithDescription("Super hero organization")
            .WithWebsite("https://justiceleague.org")
            .Build();

        Guid creatorUserId = Guid.NewGuid();
        _userContextMock.UserId.Returns(creatorUserId);

        Organization? addedOrganization = null;

        _repositoryMock.AddAsync(Arg.Do<Organization>(organization => addedOrganization = organization),
            Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        _queryServiceMock.GetOrganizationDetailsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(call => BasicOrganizationResponseTestBuilder.AResponse().WithId(call.Arg<Guid>()).Build());

        // Act
        Result<BasicOrganizationResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBe(Guid.Empty);

        addedOrganization.Should().NotBeNull();
        addedOrganization!.Name.Should().Be(command.Name);
        addedOrganization.Description.Should().Be(command.Description);
        addedOrganization.Website.Should().Be(command.Website);
        addedOrganization.Invitations.Should().ContainSingle();
        addedOrganization.Invitations.Single().UserId.Should().Be(creatorUserId);
        addedOrganization.Invitations.Single().Status.Should().Be(InvitationStatus.Accepted);

        await _repositoryMock.Received(1).AddAsync(Arg.Any<Organization>(), Arg.Any<CancellationToken>());
        await _repositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _queryServiceMock.Received(1)
            .GetOrganizationDetailsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProjectionIsNotFound()
    {
        // Arrange
        CreateOrganizationCommand command = CreateOrganizationCommandTestBuilder.ACommand().Build();

        _userContextMock.UserId.Returns(Guid.NewGuid());

        _queryServiceMock.GetOrganizationDetailsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((BasicOrganizationResponse?)null);

        // Act
        Result<BasicOrganizationResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(OrganizationErrors.OrganizationNotFound);

        await _repositoryMock.Received(1).AddAsync(Arg.Any<Organization>(), Arg.Any<CancellationToken>());
        await _repositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
