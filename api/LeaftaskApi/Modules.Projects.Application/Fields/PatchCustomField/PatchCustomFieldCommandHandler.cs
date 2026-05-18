using BuildingBlocks.Application.Commands;
using BuildingBlocks.Domain.Result;
using Modules.Projects.Application.Fields.GetProjectCustomFields;
using Modules.Projects.Domain.Entities.Field;
using Modules.Projects.Domain.Errors;
using Modules.Projects.Domain.Repositories;

namespace Modules.Projects.Application.Fields.PatchCustomField;

public sealed class PatchCustomFieldCommandHandler(IProjectFieldRepository fieldRepository)
    : ICommandHandler<PatchCustomFieldCommand, Result<CustomFieldDto>>
{
    private static readonly HashSet<string> SelectionTypeNames = ["Selección", "Selección Múltiple"];

    public async Task<Result<CustomFieldDto>> Handle(
        PatchCustomFieldCommand command,
        CancellationToken cancellationToken)
    {
        ProjectField? projectField = await fieldRepository.GetByIdTrackedAsync(
            command.ProjectId, command.FieldId, cancellationToken);

        if (projectField is null)
        {
            return Result.Failure<CustomFieldDto>(ProjectErrors.CustomFieldNotFound);
        }

        FieldType fieldType = projectField.Field.FieldType;

        if (command.TypeId.HasValue)
        {
            FieldType? newFieldType = await fieldRepository.GetFieldTypeByIdAsync(command.TypeId.Value, cancellationToken);
            if (newFieldType is null)
            {
                return Result.Failure<CustomFieldDto>(ProjectErrors.FieldTypeNotFound);
            }

            fieldType = newFieldType;
            projectField.Field.UpdateType(newFieldType);
        }

        string newName = command.Name ?? projectField.Name;
        bool newRequired = command.Required ?? !projectField.Optional;

        projectField.Update(newName, !newRequired);
        projectField.Field.UpdateIsOptional(!newRequired);

        if (command.AppliesTo is not null)
        {
            List<WorkItemTypeReadModel> workItemTypes = await fieldRepository.GetWorkItemTypesByIdsAsync(
                command.AppliesTo, cancellationToken);
            projectField.Field.SetAppliesTo(workItemTypes);
        }

        List<CustomFieldOptionDto> optionDtos;

        if (command.Options is not null)
        {
            if (SelectionTypeNames.Contains(fieldType.Name) && command.Options.Count == 0)
            {
                return Result.Failure<CustomFieldDto>(ProjectErrors.OptionsRequired);
            }

            List<Option> oldOptions = await fieldRepository.GetOptionsTrackedByFieldIdAsync(
                projectField.Field.Id, cancellationToken);
            fieldRepository.RemoveOptions(oldOptions);

            List<Option> newOptions = command.Options
                .Select(value => new Option(Guid.NewGuid(), value, projectField.Field))
                .ToList();
            await fieldRepository.AddOptionsAsync(newOptions, cancellationToken);

            optionDtos = newOptions
                .Select(o => new CustomFieldOptionDto(o.Id, o.Name))
                .ToList();
        }
        else
        {
            List<Option> existingOptions = await fieldRepository.GetOptionsTrackedByFieldIdAsync(
                projectField.Field.Id, cancellationToken);
            optionDtos = existingOptions
                .Select(o => new CustomFieldOptionDto(o.Id, o.Name))
                .ToList();
        }

        await fieldRepository.SaveChangesAsync(cancellationToken);

        List<CustomFieldWorkItemTypeDto> workItemTypeDtos = projectField.Field.AppliesTo
            .Select(wt => new CustomFieldWorkItemTypeDto(wt.Id, wt.Name))
            .ToList();

        return Result.Success(new CustomFieldDto(
            projectField.Id,
            projectField.Name,
            fieldType.Id,
            optionDtos,
            !projectField.Optional,
            workItemTypeDtos));
    }
}
