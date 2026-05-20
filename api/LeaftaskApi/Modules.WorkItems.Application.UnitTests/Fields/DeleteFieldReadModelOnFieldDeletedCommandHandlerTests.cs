using FluentAssertions;
using Modules.WorkItems.Application.Fields.DeleteFieldReadModelOnFieldDeleted;
using Modules.WorkItems.Domain.Entities.Field;
using Modules.WorkItems.Domain.Repositories;
using NSubstitute;

namespace Modules.WorkItems.Application.UnitTests.Fields;

public class DeleteFieldReadModelOnFieldDeletedCommandHandlerTests
{
    private readonly DeleteFieldReadModelOnFieldDeletedCommandHandler _handler;
    private readonly IFieldRepository _fieldRepositoryMock;

    public DeleteFieldReadModelOnFieldDeletedCommandHandlerTests()
    {
        _fieldRepositoryMock = Substitute.For<IFieldRepository>();
        _handler = new DeleteFieldReadModelOnFieldDeletedCommandHandler(_fieldRepositoryMock);
    }

    [Fact]
    public async Task Handle_Should_RemoveFieldReadModel_When_Exists()
    {
        // Arrange
        Guid fieldId = Guid.NewGuid();
        DeleteFieldReadModelOnFieldDeletedCommand command = new(fieldId);

        FieldTypeReadModel fieldType = new(Guid.NewGuid(), "Text");
        FieldReadModel fieldReadModel = new(fieldId, "Field", true, fieldType);
        _fieldRepositoryMock.GetFieldReadModelTrackedByIdAsync(fieldId, Arg.Any<CancellationToken>())
            .Returns(fieldReadModel);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _fieldRepositoryMock.Received(1).RemoveFieldReadModel(fieldReadModel);
        await _fieldRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_Skip_When_FieldReadModelNotFound()
    {
        // Arrange
        Guid fieldId = Guid.NewGuid();
        DeleteFieldReadModelOnFieldDeletedCommand command = new(fieldId);

        _fieldRepositoryMock.GetFieldReadModelTrackedByIdAsync(fieldId, Arg.Any<CancellationToken>())
            .Returns((FieldReadModel?)null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _fieldRepositoryMock.DidNotReceive().RemoveFieldReadModel(Arg.Any<FieldReadModel>());
        await _fieldRepositoryMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
