using Modules.Projects.Domain.Entities.Field;

namespace Modules.Projects.Domain.Repositories;

public interface IProjectFieldRepository
{
    Task<FieldType?> GetFieldTypeByIdAsync(Guid fieldTypeId, CancellationToken cancellationToken = default);
    Task<List<WorkItemTypeReadModel>> GetWorkItemTypesByIdsAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default);
    Task AddFieldAsync(Field field, CancellationToken cancellationToken = default);
    Task AddOptionsAsync(IEnumerable<Option> options, CancellationToken cancellationToken = default);
    Task<List<Option>> GetOptionsTrackedByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default);
    void RemoveOptions(IEnumerable<Option> options);
    Task AddAsync(ProjectField field, CancellationToken cancellationToken = default);
    Task<ProjectField?> GetByIdTrackedAsync(Guid projectId, Guid fieldId, CancellationToken cancellationToken = default);
    void Remove(ProjectField field);
    void RemoveField(Field field);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
