using System.Globalization;
using BuildingBlocks.Domain.Result;
using Modules.WorkItems.Domain.Entities;
using Modules.WorkItems.Domain.Entities.Field;
using Modules.WorkItems.Domain.Errors;
using Modules.WorkItems.Domain.Repositories;

namespace Modules.WorkItems.Application.WorkItems;

internal static class WorkItemCustomFieldsService
{
    internal static async Task<Result<List<WorkItemChange>>> ApplyAsync(
        WorkItem workItem,
        IReadOnlyDictionary<Guid, string> customFields,
        IFieldRepository fieldRepository,
        CancellationToken ct)
    {
        if (customFields.Count == 0)
        {
            return Result.Success(new List<WorkItemChange>());
        }

        List<FieldValue> existing = await fieldRepository.GetFieldValuesForWorkItemAsync(workItem.Id, ct);
        List<WorkItemChange> changes = [];

        foreach ((Guid fieldId, string newValue) in customFields)
        {
            FieldReadModel? field = existing.FirstOrDefault(fv => fv.Field.Id == fieldId)?.Field
                ?? await fieldRepository.GetFieldReadModelTrackedByIdAsync(fieldId, ct);

            if (field is null)
            {
                continue;
            }

            if (!field.IsOptional && string.IsNullOrWhiteSpace(newValue))
            {
                return Result.Failure<List<WorkItemChange>>(WorkItemErrors.RequiredFieldValueMissing);
            }

            Result typeCheck = ValidateValue(field.FieldType.Name, newValue);
            if (typeCheck.IsFailure)
            {
                return Result.Failure<List<WorkItemChange>>(WorkItemErrors.InvalidFieldValue);
            }

            FieldValue? existingFv = existing.FirstOrDefault(fv => fv.Field.Id == fieldId);
            if (existingFv is not null)
            {
                if (existingFv.Value != newValue)
                {
                    changes.Add(new WorkItemChange(field.Name, existingFv.Value, newValue));
                    existingFv.UpdateValue(newValue);
                }
            }
            else
            {
                FieldValue fv = new(Guid.NewGuid(), field, workItem, newValue);
                await fieldRepository.AddFieldValueAsync(fv, ct);
                changes.Add(new WorkItemChange(field.Name, string.Empty, newValue));
            }
        }

        return Result.Success(changes);
    }

    private static Result ValidateValue(string typeName, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Success();
        }

        return typeName switch
        {
            "Número" => decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _)
                ? Result.Success()
                : Result.Failure(WorkItemErrors.InvalidFieldValue),

            "Fecha" => DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
                ? Result.Success()
                : Result.Failure(WorkItemErrors.InvalidFieldValue),

            "Casilla de Verificación" => value is "true" or "false"
                ? Result.Success()
                : Result.Failure(WorkItemErrors.InvalidFieldValue),

            _ => Result.Success()
        };
    }
}
