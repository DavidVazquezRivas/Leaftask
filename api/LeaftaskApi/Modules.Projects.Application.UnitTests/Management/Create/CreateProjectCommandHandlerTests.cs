using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Management.Create;
using Modules.Projects.Application.UnitTests.TestBuilders;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Owner;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Management.Create;

public class CreateProjectCommandHandlerTests
{
    private readonly CreateProjectCommandHandler _handler;
    private readonly IProjectRepository _projectRepositoryMock;
    private readonly IUserReadModelRepository _userReadModelRepositoryMock;
    private readonly IOrganizationReadModelRepository _organizationReadModelRepositoryMock;
    private readonly IUserContext _userContextMock;

    public CreateProjectCommandHandlerTests()
    {
        _projectRepositoryMock = Substitute.For<IProjectRepository>();
        _userReadModelRepositoryMock = Substitute.For<IUserReadModelRepository>();
        _organizationReadModelRepositoryMock = Substitute.For<IOrganizationReadModelRepository>();
        _userContextMock = Substitute.For<IUserContext>();

        _handler = new CreateProjectCommandHandler(
            _projectRepositoryMock,
            _userReadModelRepositoryMock,
            _organizationReadModelRepositoryMock,
            _userContextMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_PersonalProjectIsCreated()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        CreateProjectCommand command = CreateProjectCommandTestBuilder.ACommand().Build();

        UserReadModel user = new(userId, "John", "Doe", "john@example.com");
        _userReadModelRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        _projectRepositoryMock.ExistsByAbbreviationAsync(command.Abbreviation, userId, cancellationToken: Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        Result<ProjectResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(command.Name);
        result.Value.Abbreviation.Should().Be(command.Abbreviation);
        result.Value.OrganizationId.Should().BeNull();
        result.Value.PrivacyLevel.Should().Be(ProjectPrivacy.Public);

        await _projectRepositoryMock.Received(1).AddAsync(Arg.Any<Project>(), Arg.Any<CancellationToken>());
        await _projectRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_OrganizationProjectIsCreated()
    {
        // Arrange
        Guid organizationId = Guid.NewGuid();
        _userContextMock.UserId.Returns(Guid.NewGuid());

        CreateProjectCommand command = CreateProjectCommandTestBuilder.ACommand()
            .WithOrganizationId(organizationId)
            .Build();

        OrganizationReadModel organization = new(organizationId);
        _organizationReadModelRepositoryMock.GetByIdAsync(organizationId, Arg.Any<CancellationToken>())
            .Returns(organization);

        _projectRepositoryMock.ExistsByAbbreviationAsync(command.Abbreviation, organizationId, cancellationToken: Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        Result<ProjectResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.OrganizationId.Should().Be(organizationId);

        await _projectRepositoryMock.Received(1).AddAsync(Arg.Any<Project>(), Arg.Any<CancellationToken>());
        await _projectRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AbbreviationAlreadyExists()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        CreateProjectCommand command = CreateProjectCommandTestBuilder.ACommand().Build();

        UserReadModel user = new(userId, "John", "Doe", "john@example.com");
        _userReadModelRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(user);

        _projectRepositoryMock.ExistsByAbbreviationAsync(command.Abbreviation, userId, cancellationToken: Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        Result<ProjectResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.DuplicatedAbbreviation);

        await _projectRepositoryMock.DidNotReceive().AddAsync(Arg.Any<Project>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_OrganizationIsNotFound()
    {
        // Arrange
        Guid organizationId = Guid.NewGuid();
        _userContextMock.UserId.Returns(Guid.NewGuid());

        CreateProjectCommand command = CreateProjectCommandTestBuilder.ACommand()
            .WithOrganizationId(organizationId)
            .Build();

        _organizationReadModelRepositoryMock.GetByIdAsync(organizationId, Arg.Any<CancellationToken>())
            .Returns((OrganizationReadModel?)null);

        // Act
        Result<ProjectResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.OwnerNotFound);

        await _projectRepositoryMock.DidNotReceive().AddAsync(Arg.Any<Project>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserIsNotFound()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        _userContextMock.UserId.Returns(userId);

        CreateProjectCommand command = CreateProjectCommandTestBuilder.ACommand().Build();

        _userReadModelRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((UserReadModel?)null);

        // Act
        Result<ProjectResponse> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.OwnerNotFound);

        await _projectRepositoryMock.DidNotReceive().AddAsync(Arg.Any<Project>(), Arg.Any<CancellationToken>());
    }
}
