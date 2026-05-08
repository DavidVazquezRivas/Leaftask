using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Fields.GetProjectCustomFields;
using Modules.Projects.Domain.Entities;
using Modules.Projects.Domain.Entities.Field;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Fields.CreateCustomField;

public sealed class CreateCustomFieldCommandHandler(
    IProjectRepository projectRepository,
    IProjectFieldRepository fieldRepository)
    : ICommandHandler<CreateCustomFieldCommand, Result<CustomFieldDto>>
{
    private static readonly HashSet<string> SelectionTypeNames = ["Selección", "Selección Múltiple"];

    public async Task<Result<CustomFieldDto>> Handle(
        CreateCustomFieldCommand command,
        CancellationToken cancellationToken)
    {
        FieldType? fieldType = await fieldRepository.GetFieldTypeByIdAsync(command.TypeId, cancellationToken);
        if (fieldType is null)
        {
            return Result.Failure<CustomFieldDto>(ProjectErrors.FieldTypeNotFound);
        }

        if (SelectionTypeNames.Contains(fieldType.Name) && command.Options.Count == 0)
        {
            return Result.Failure<CustomFieldDto>(ProjectErrors.OptionsRequired);
        }

        Project? project = await projectRepository.GetByIdTrackedAsync(command.ProjectId, cancellationToken);
        if (project is null)
        {
            return Result.Failure<CustomFieldDto>(ProjectErrors.ProjectNotFound);
        }

        Field field = new(Guid.NewGuid(), !command.Required, command.Name, fieldType);
        await fieldRepository.AddFieldAsync(field, cancellationToken);

        List<Option> options = command.Options
            .Select(value => new Option(Guid.NewGuid(), value, field))
            .ToList();

        if (options.Count > 0)
        {
            await fieldRepository.AddOptionsAsync(options, cancellationToken);
        }

        ProjectField projectField = new(Guid.NewGuid(), command.Name, false, !command.Required, field, project);
        await fieldRepository.AddAsync(projectField, cancellationToken);

        await fieldRepository.SaveChangesAsync(cancellationToken);

        List<CustomFieldOptionDto> optionDtos = options
            .Select(o => new CustomFieldOptionDto(o.Id, o.Name))
            .ToList();

        return Result.Success(new CustomFieldDto(
            projectField.Id,
            projectField.Name,
            fieldType.Id,
            optionDtos,
            command.Required,
            []));
    }
}
