using FluentAssertions;
using Modules.WorkItems.Application.Fields.CreateFieldReadModelOnFieldCreated;
using Modules.WorkItems.Domain.Entities.Field;
using Modules.WorkItems.Domain.Entities.Properties;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.Fields;

public class CreateFieldReadModelOnFieldCreatedCommandHandlerTests
{
    private readonly CreateFieldReadModelOnFieldCreatedCommandHandler _handler;
    private readonly IFieldRepository _fieldRepositoryMock;
    private readonly IWorkItemConfigurationRepository _configurationRepositoryMock;

    public CreateFieldReadModelOnFieldCreatedCommandHandlerTests()
    {
        _fieldRepositoryMock = Substitute.For<IFieldRepository>();
        _configurationRepositoryMock = Substitute.For<IWorkItemConfigurationRepository>();

        _handler = new CreateFieldReadModelOnFieldCreatedCommandHandler(
            _fieldRepositoryMock,
            _configurationRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_CreateFieldReadModel_When_NotExists()
    {
        // Arrange
        Guid fieldId = Guid.NewGuid();
        Guid fieldTypeId = Guid.NewGuid();
        Guid typeId = Guid.NewGuid();
        CreateFieldReadModelOnFieldCreatedCommand command = new(
            fieldId, "Reviewer", true, fieldTypeId, [typeId]);

        _fieldRepositoryMock.GetFieldReadModelByIdAsync(fieldId, Arg.Any<CancellationToken>())
            .Returns((FieldReadModel?)null);

        FieldTypeReadModel fieldType = new(fieldTypeId, "Text");
        _fieldRepositoryMock.GetFieldTypeReadModelByIdAsync(fieldTypeId, Arg.Any<CancellationToken>())
            .Returns(fieldType);

        WorkItemType workItemType = new(typeId, "Task");
        _configurationRepositoryMock.GetTypesByIdsAsync(Arg.Is<IReadOnlyList<Guid>>(ids => ids.Contains(typeId)), Arg.Any<CancellationToken>())
            .Returns([workItemType]);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _fieldRepositoryMock.Received(1).AddFieldReadModelAsync(
            Arg.Is<FieldReadModel>(f => f.Id == fieldId && f.Name == "Reviewer"),
            Arg.Any<CancellationToken>());
        await _fieldRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_Skip_When_FieldReadModelAlreadyExists()
    {
        // Arrange
        Guid fieldId = Guid.NewGuid();
        Guid fieldTypeId = Guid.NewGuid();
        CreateFieldReadModelOnFieldCreatedCommand command = new(fieldId, "Name", false, fieldTypeId, []);

        FieldTypeReadModel fieldType = new(fieldTypeId, "Text");
        FieldReadModel existingReadModel = new(fieldId, "Name", false, fieldType);
        _fieldRepositoryMock.GetFieldReadModelByIdAsync(fieldId, Arg.Any<CancellationToken>())
            .Returns(existingReadModel);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _fieldRepositoryMock.DidNotReceive().AddFieldReadModelAsync(
            Arg.Any<FieldReadModel>(), Arg.Any<CancellationToken>());
        await _fieldRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_Skip_When_FieldTypeReadModelNotFound()
    {
        // Arrange
        Guid fieldId = Guid.NewGuid();
        Guid fieldTypeId = Guid.NewGuid();
        CreateFieldReadModelOnFieldCreatedCommand command = new(fieldId, "Name", false, fieldTypeId, []);

        _fieldRepositoryMock.GetFieldReadModelByIdAsync(fieldId, Arg.Any<CancellationToken>())
            .Returns((FieldReadModel?)null);
        _fieldRepositoryMock.GetFieldTypeReadModelByIdAsync(fieldTypeId, Arg.Any<CancellationToken>())
            .Returns((FieldTypeReadModel?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _fieldRepositoryMock.DidNotReceive().AddFieldReadModelAsync(
            Arg.Any<FieldReadModel>(), Arg.Any<CancellationToken>());
    }
}
