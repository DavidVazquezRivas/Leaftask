using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Fields.GetProjectCustomFields;
using Modules.Projects.Application.Fields.PatchCustomField;
using Modules.Projects.Domain.Entities.Field;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Fields.PatchCustomField;

public class PatchCustomFieldCommandHandlerTests
{
    private readonly PatchCustomFieldCommandHandler _handler;
    private readonly IProjectFieldRepository _fieldRepositoryMock;

    public PatchCustomFieldCommandHandlerTests()
    {
        _fieldRepositoryMock = Substitute.For<IProjectFieldRepository>();
        _handler = new PatchCustomFieldCommandHandler(_fieldRepositoryMock);
    }

    private static ProjectField CreateProjectField(Guid fieldId, string name, bool optional = false)
    {
        FieldType fieldType = new(Guid.NewGuid(), "Text", "A text field");
        Field field = new(fieldId, optional, name, fieldType);
        return new ProjectField(Guid.NewGuid(), name, false, optional, field, ProjectTestBuilder.AProject().Build());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProjectFieldNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid fieldId = Guid.NewGuid();
        PatchCustomFieldCommand command = new(projectId, fieldId, "NewName", null, null, null, null);

        _fieldRepositoryMock.GetByIdTrackedAsync(projectId, fieldId, Arg.Any<CancellationToken>())
            .Returns((ProjectField?)null);

        // Act
        Result<CustomFieldDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.CustomFieldNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_NameIsUpdated()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid fieldId = Guid.NewGuid();
        PatchCustomFieldCommand command = new(projectId, fieldId, "NewName", null, null, null, null);

        ProjectField projectField = CreateProjectField(fieldId, "OldName");
        _fieldRepositoryMock.GetByIdTrackedAsync(projectId, fieldId, Arg.Any<CancellationToken>())
            .Returns(projectField);
        _fieldRepositoryMock.GetOptionsTrackedByFieldIdAsync(projectField.Field.Id, Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        Result<CustomFieldDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("NewName");
        await _fieldRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NewFieldTypeNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid fieldId = Guid.NewGuid();
        Guid newTypeId = Guid.NewGuid();
        PatchCustomFieldCommand command = new(projectId, fieldId, null, newTypeId, null, null, null);

        ProjectField projectField = CreateProjectField(fieldId, "FieldName");
        _fieldRepositoryMock.GetByIdTrackedAsync(projectId, fieldId, Arg.Any<CancellationToken>())
            .Returns(projectField);
        _fieldRepositoryMock.GetFieldTypeByIdAsync(newTypeId, Arg.Any<CancellationToken>())
            .Returns((FieldType?)null);

        // Act
        Result<CustomFieldDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.FieldTypeNotFound);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_SelectionTypeRequiresOptions()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid fieldId = Guid.NewGuid();
        Guid newTypeId = Guid.NewGuid();
        PatchCustomFieldCommand command = new(projectId, fieldId, null, newTypeId, [], null, null);

        ProjectField projectField = CreateProjectField(fieldId, "FieldName");
        _fieldRepositoryMock.GetByIdTrackedAsync(projectId, fieldId, Arg.Any<CancellationToken>())
            .Returns(projectField);

        FieldType selectionType = new(newTypeId, "Selección", "A selection field");
        _fieldRepositoryMock.GetFieldTypeByIdAsync(newTypeId, Arg.Any<CancellationToken>())
            .Returns(selectionType);
        _fieldRepositoryMock.GetOptionsTrackedByFieldIdAsync(projectField.Field.Id, Arg.Any<CancellationToken>())
            .Returns([]);

        // Act
        Result<CustomFieldDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.OptionsRequired);
    }

    [Fact]
    public async Task Handle_Should_ReplaceOptions_When_OptionsProvided()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid fieldId = Guid.NewGuid();
        PatchCustomFieldCommand command = new(projectId, fieldId, null, null, ["Option A", "Option B"], null, null);

        FieldType selectionType = new(Guid.NewGuid(), "Selección", "A selection field");
        Field field = new(fieldId, false, "FieldName", selectionType);
        ProjectField projectField = new(Guid.NewGuid(), "FieldName", false, false, field, ProjectTestBuilder.AProject().Build());

        _fieldRepositoryMock.GetByIdTrackedAsync(projectId, fieldId, Arg.Any<CancellationToken>())
            .Returns(projectField);

        List<Option> existingOptions = [new Option(Guid.NewGuid(), "Old Option", field)];
        _fieldRepositoryMock.GetOptionsTrackedByFieldIdAsync(field.Id, Arg.Any<CancellationToken>())
            .Returns(existingOptions);

        // Act
        Result<CustomFieldDto> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Options.Should().HaveCount(2);
        _fieldRepositoryMock.Received(1).RemoveOptions(existingOptions);
        await _fieldRepositoryMock.Received(1).AddOptionsAsync(
            Arg.Is<IEnumerable<Option>>(opts => opts.Count() == 2),
            Arg.Any<CancellationToken>());
    }
}
