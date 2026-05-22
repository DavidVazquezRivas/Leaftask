using Modules.WorkItems.Domain.Entities.Field;

namespace Modules.WorkItems.Domain.Repositories;

public interface IFieldRepository
{
    Task<FieldReadModel?> GetFieldReadModelByIdAsync(Guid fieldId, CancellationToken cancellationToken = default);
    Task<FieldReadModel?> GetFieldReadModelTrackedByIdAsync(Guid fieldId, CancellationToken cancellationToken = default);
    Task<FieldTypeReadModel?> GetFieldTypeReadModelByIdAsync(Guid fieldTypeId, CancellationToken cancellationToken = default);
    Task<List<FieldValue>> GetFieldValuesForWorkItemAsync(Guid workItemId, CancellationToken cancellationToken = default);
    Task AddFieldValueAsync(FieldValue fieldValue, CancellationToken cancellationToken = default);
    Task AddFieldReadModelAsync(FieldReadModel fieldReadModel, CancellationToken cancellationToken = default);
    void RemoveFieldReadModel(FieldReadModel fieldReadModel);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
