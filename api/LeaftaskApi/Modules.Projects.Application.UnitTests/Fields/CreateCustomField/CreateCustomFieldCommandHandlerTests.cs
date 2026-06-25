using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Fields.CreateCustomField;
using Modules.Projects.Application.Fields.GetProjectCustomFields;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Field;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Fields.CreateCustomField;

public class CreateCustomFieldCommandHandlerTests
{
    private readonly CreateCustomFieldCommandHandler _handler;
    private readonly IProjectRepository _projectRepositoryMock;
    private readonly IProjectFieldRepository _fieldRepositoryMock;

    public CreateCustomFieldCommandHandlerTests()
    {
        _projectRepositoryMock = Substitute.For<IProjectRepository>();
        _fieldRepositoryMock = Substitute.For<IProjectFieldRepository>();

        _handler = new CreateCustomFieldCommandHandler(
            _projectRepositoryMock,
            _fieldRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_FieldIsCreated()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid typeId = Guid.NewGuid();
        CreateCustomFieldCommand command = new(projectId, "Priority", typeId, [], true, []);

        FieldType fieldType = new(typeId, "Text", "A text field");
        _fieldRepositoryMock.GetFieldTypeByIdAsync(typeId, Arg.Any<CancellationToken>()).Returns(fieldType);

        Project project = ProjectTestBuilder.AProject().Build();
        _projectRepositoryMock.GetByIdTrackedAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);

        // Act
        Result<CustomFieldDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Priority");
        result.Value.Required.Should().BeTrue();
        await _fieldRepositoryMock.Received(1).AddFieldAsync(Arg.Any<Field>(), Arg.Any<CancellationToken>());
        await _fieldRepositoryMock.Received(1).AddAsync(Arg.Any<ProjectField>(), Arg.Any<CancellationToken>());
        await _fieldRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_FieldTypeNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid typeId = Guid.NewGuid();
        CreateCustomFieldCommand command = new(projectId, "Priority", typeId, [], true, []);

        _fieldRepositoryMock.GetFieldTypeByIdAsync(typeId, Arg.Any<CancellationToken>())
            .Returns((FieldType?)null);

        // Act
        Result<CustomFieldDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.FieldTypeNotFound);
        await _fieldRepositoryMock.DidNotReceive().AddFieldAsync(Arg.Any<Field>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_SelectionFieldHasNoOptions()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid typeId = Guid.NewGuid();
        CreateCustomFieldCommand command = new(projectId, "Status", typeId, [], true, []);

        FieldType selectionType = new(typeId, "Selección", "A selection field");
        _fieldRepositoryMock.GetFieldTypeByIdAsync(typeId, Arg.Any<CancellationToken>()).Returns(selectionType);

        // Act
        Result<CustomFieldDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.OptionsRequired);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProjectNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid typeId = Guid.NewGuid();
        CreateCustomFieldCommand command = new(projectId, "Priority", typeId, [], true, []);

        FieldType fieldType = new(typeId, "Text", "A text field");
        _fieldRepositoryMock.GetFieldTypeByIdAsync(typeId, Arg.Any<CancellationToken>()).Returns(fieldType);

        _projectRepositoryMock.GetByIdTrackedAsync(projectId, Arg.Any<CancellationToken>())
            .Returns((Project?)null);

        // Act
        Result<CustomFieldDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.ProjectNotFound);
    }

    [Fact]
    public async Task Handle_Should_CreateFieldWithOptions_When_SelectionTypeWithOptions()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid typeId = Guid.NewGuid();
        CreateCustomFieldCommand command = new(projectId, "Status", typeId, ["Open", "In Progress", "Done"], true, []);

        FieldType selectionType = new(typeId, "Selección", "A selection field");
        _fieldRepositoryMock.GetFieldTypeByIdAsync(typeId, Arg.Any<CancellationToken>()).Returns(selectionType);

        Project project = ProjectTestBuilder.AProject().Build();
        _projectRepositoryMock.GetByIdTrackedAsync(projectId, Arg.Any<CancellationToken>()).Returns(project);

        // Act
        Result<CustomFieldDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Options.Should().HaveCount(3);
        await _fieldRepositoryMock.Received(1).AddOptionsAsync(
            Arg.Is<IEnumerable<Option>>(opts => opts.Count() == 3),
            Arg.Any<CancellationToken>());
    }
}
