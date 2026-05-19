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

        List<WorkItemTypeReadModel> workItemTypes = [];
        if (command.AppliesTo.Count > 0)
        {
            workItemTypes = await fieldRepository.GetWorkItemTypesByIdsAsync(command.AppliesTo, cancellationToken);
        }

        Field field = new(Guid.NewGuid(), !command.Required, command.Name, fieldType);
        field.SetAppliesTo(workItemTypes);
        field.NotifyCreated();
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

        List<CustomFieldWorkItemTypeDto> workItemTypeDtos = workItemTypes
            .Select(wt => new CustomFieldWorkItemTypeDto(wt.Id, wt.Name))
            .ToList();

        return Result.Success(new CustomFieldDto(
            field.Id,
            projectField.Name,
            fieldType.Id,
            optionDtos,
            command.Required,
            workItemTypeDtos));
    }
}
