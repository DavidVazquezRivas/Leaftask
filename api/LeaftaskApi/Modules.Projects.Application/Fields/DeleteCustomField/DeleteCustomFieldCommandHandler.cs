using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Domain.Entities.Field;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Fields.DeleteCustomField;

public sealed class DeleteCustomFieldCommandHandler(IProjectFieldRepository fieldRepository)
    : ICommandHandler<DeleteCustomFieldCommand, Result>
{
    public async Task<Result> Handle(DeleteCustomFieldCommand command, CancellationToken cancellationToken)
    {
        ProjectField? projectField = await fieldRepository.GetByIdTrackedAsync(
            command.ProjectId, command.FieldId, cancellationToken);

        if (projectField is null)
        {
            return Result.Failure(ProjectErrors.CustomFieldNotFound);
        }

        Field field = projectField.Field;

        field.NotifyDeleted();
        fieldRepository.Remove(projectField);
        fieldRepository.RemoveField(field);

        await fieldRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
