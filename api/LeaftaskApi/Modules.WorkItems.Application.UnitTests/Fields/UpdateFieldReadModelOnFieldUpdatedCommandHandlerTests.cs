using FluentAssertions;
using Modules.WorkItems.Application.Fields.UpdateFieldReadModelOnFieldUpdated;
using Modules.WorkItems.Domain.Entities.Field;
using Modules.WorkItems.Domain.Entities.Properties;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.Fields;

public class UpdateFieldReadModelOnFieldUpdatedCommandHandlerTests
{
    private readonly UpdateFieldReadModelOnFieldUpdatedCommandHandler _handler;
    private readonly IFieldRepository _fieldRepositoryMock;
    private readonly IWorkItemConfigurationRepository _configurationRepositoryMock;

    public UpdateFieldReadModelOnFieldUpdatedCommandHandlerTests()
    {
        _fieldRepositoryMock = Substitute.For<IFieldRepository>();
        _configurationRepositoryMock = Substitute.For<IWorkItemConfigurationRepository>();

        _handler = new UpdateFieldReadModelOnFieldUpdatedCommandHandler(
            _fieldRepositoryMock,
            _configurationRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_UpdateExistingFieldReadModel()
    {
        // Arrange
        Guid fieldId = Guid.NewGuid();
        Guid fieldTypeId = Guid.NewGuid();
        UpdateFieldReadModelOnFieldUpdatedCommand command = new(fieldId, "Updated Name", false, fieldTypeId, []);

        FieldTypeReadModel fieldType = new(fieldTypeId, "Text");
        _fieldRepositoryMock.GetFieldTypeReadModelByIdAsync(fieldTypeId, Arg.Any<CancellationToken>())
            .Returns(fieldType);

        _configurationRepositoryMock.GetTypesByIdsAsync(Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>())
            .Returns([]);

        FieldReadModel existingReadModel = new(fieldId, "Old Name", true, fieldType);
        _fieldRepositoryMock.GetFieldReadModelTrackedByIdAsync(fieldId, Arg.Any<CancellationToken>())
            .Returns(existingReadModel);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingReadModel.Name.Should().Be("Updated Name");
        existingReadModel.IsOptional.Should().BeFalse();
        await _fieldRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _fieldRepositoryMock.DidNotReceive().AddFieldReadModelAsync(Arg.Any<FieldReadModel>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_CreateFieldReadModel_When_NotExists()
    {
        // Arrange
        Guid fieldId = Guid.NewGuid();
        Guid fieldTypeId = Guid.NewGuid();
        UpdateFieldReadModelOnFieldUpdatedCommand command = new(fieldId, "New Name", true, fieldTypeId, []);

        FieldTypeReadModel fieldType = new(fieldTypeId, "Text");
        _fieldRepositoryMock.GetFieldTypeReadModelByIdAsync(fieldTypeId, Arg.Any<CancellationToken>())
            .Returns(fieldType);

        _configurationRepositoryMock.GetTypesByIdsAsync(Arg.Any<IReadOnlyList<Guid>>(), Arg.Any<CancellationToken>())
            .Returns([]);

        _fieldRepositoryMock.GetFieldReadModelTrackedByIdAsync(fieldId, Arg.Any<CancellationToken>())
            .Returns((FieldReadModel?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _fieldRepositoryMock.Received(1).AddFieldReadModelAsync(
            Arg.Is<FieldReadModel>(f => f.Id == fieldId && f.Name == "New Name"),
            Arg.Any<CancellationToken>());
        await _fieldRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_Skip_When_FieldTypeReadModelNotFound()
    {
        // Arrange
        Guid fieldId = Guid.NewGuid();
        Guid fieldTypeId = Guid.NewGuid();
        UpdateFieldReadModelOnFieldUpdatedCommand command = new(fieldId, "Name", false, fieldTypeId, []);

        _fieldRepositoryMock.GetFieldTypeReadModelByIdAsync(fieldTypeId, Arg.Any<CancellationToken>())
            .Returns((FieldTypeReadModel?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _fieldRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
