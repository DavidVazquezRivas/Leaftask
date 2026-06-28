using BuildingBlocks.Domain.Result;
using FluentAssertions;
using Modules.Projects.Application.Fields.DeleteCustomField;
using Modules.Projects.Domain.Entities.Field;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.Domain.UnitTests.TestBuilders;
using NSubstitute;

namespace Modules.Projects.Application.UnitTests.Fields.DeleteCustomField;

public class DeleteCustomFieldCommandHandlerTests
{
    private readonly DeleteCustomFieldCommandHandler _handler;
    private readonly IProjectFieldRepository _fieldRepositoryMock;

    public DeleteCustomFieldCommandHandlerTests()
    {
        _fieldRepositoryMock = Substitute.For<IProjectFieldRepository>();
        _handler = new DeleteCustomFieldCommandHandler(_fieldRepositoryMock);
    }

    private static ProjectField CreateProjectField(Guid fieldId, string name)
    {
        FieldType fieldType = new(Guid.NewGuid(), "Text", "A text field");
        Field field = new(fieldId, false, name, fieldType);
        return new ProjectField(Guid.NewGuid(), name, false, false, field, ProjectTestBuilder.AProject().Build());
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_FieldIsDeleted()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid fieldId = Guid.NewGuid();
        DeleteCustomFieldCommand command = new(projectId, fieldId);

        ProjectField projectField = CreateProjectField(fieldId, "Priority");
        _fieldRepositoryMock.GetByIdTrackedAsync(projectId, fieldId, Arg.Any<CancellationToken>())
            .Returns(projectField);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _fieldRepositoryMock.Received(1).Remove(projectField);
        _fieldRepositoryMock.Received(1).RemoveField(projectField.Field);
        await _fieldRepositoryMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_FieldNotFound()
    {
        // Arrange
        Guid projectId = Guid.NewGuid();
        Guid fieldId = Guid.NewGuid();
        DeleteCustomFieldCommand command = new(projectId, fieldId);

        _fieldRepositoryMock.GetByIdTrackedAsync(projectId, fieldId, Arg.Any<CancellationToken>())
            .Returns((ProjectField?)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ProjectErrors.CustomFieldNotFound);
        _fieldRepositoryMock.DidNotReceive().Remove(Arg.Any<ProjectField>());
        _fieldRepositoryMock.DidNotReceive().RemoveField(Arg.Any<Field>());
    }
}
