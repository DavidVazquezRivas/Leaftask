using Microsoft.EntityFrameworkCore;
using Modules.WorkItems.Domain.Entities.Field;
using Modules.WorkItems.Domain.Repositories;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivenInfrastructure.Repositories;

public sealed class FieldRepository(WorkItemsDbContext dbContext) : IFieldRepository
{
    public async Task<FieldReadModel?> GetFieldReadModelByIdAsync(
        Guid fieldId,
        CancellationToken cancellationToken = default) =>
        await dbContext.FieldReadModels
            .AsNoTracking()
            .Include(f => f.FieldType)
            .Include(f => f.AppliesTo)
            .FirstOrDefaultAsync(f => f.Id == fieldId, cancellationToken);

    public async Task<FieldReadModel?> GetFieldReadModelTrackedByIdAsync(
        Guid fieldId,
        CancellationToken cancellationToken = default) =>
        await dbContext.FieldReadModels
            .Include(f => f.FieldType)
            .Include(f => f.AppliesTo)
            .FirstOrDefaultAsync(f => f.Id == fieldId, cancellationToken);

    public async Task<FieldTypeReadModel?> GetFieldTypeReadModelByIdAsync(
        Guid fieldTypeId,
        CancellationToken cancellationToken = default) =>
        await dbContext.FieldTypeReadModels
            .FirstOrDefaultAsync(ft => ft.Id == fieldTypeId, cancellationToken);

    public async Task<List<FieldValue>> GetFieldValuesForWorkItemAsync(
        Guid workItemId,
        CancellationToken cancellationToken = default) =>
        await dbContext.FieldValues
            .Include(fv => fv.Field)
                .ThenInclude(f => f.FieldType)
            .Where(fv => EF.Property<Guid>(fv, "work_item_id") == workItemId)
            .ToListAsync(cancellationToken);

    public async Task AddFieldValueAsync(FieldValue fieldValue, CancellationToken cancellationToken = default) =>
        await dbContext.FieldValues.AddAsync(fieldValue, cancellationToken);

    public async Task AddFieldReadModelAsync(FieldReadModel fieldReadModel, CancellationToken cancellationToken = default) =>
        await dbContext.FieldReadModels.AddAsync(fieldReadModel, cancellationToken);

    public void RemoveFieldReadModel(FieldReadModel fieldReadModel) =>
        dbContext.FieldReadModels.Remove(fieldReadModel);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
