using Modules.WorkItems.Domain.Entities.Field;

namespace Modules.WorkItems.Domain.Repositories;

public interface IFieldRepository
{
    Task<FieldReadModel?> GetFieldReadModelByIdAsync(Guid fieldId, CancellationToken cancellationToken = default);
    Task<List<FieldValue>> GetFieldValuesForWorkItemAsync(Guid workItemId, CancellationToken cancellationToken = default);
    Task AddFieldValueAsync(FieldValue fieldValue, CancellationToken cancellationToken = default);
}
