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
            .FirstOrDefaultAsync(f => f.Id == fieldId, cancellationToken);

    public async Task<List<FieldValue>> GetFieldValuesForWorkItemAsync(
        Guid workItemId,
        CancellationToken cancellationToken = default) =>
        await dbContext.FieldValues
            .Include(fv => fv.Field)
            .Where(fv => EF.Property<Guid>(fv, "work_item_id") == workItemId)
            .ToListAsync(cancellationToken);

    public async Task AddFieldValueAsync(FieldValue fieldValue, CancellationToken cancellationToken = default) =>
        await dbContext.FieldValues.AddAsync(fieldValue, cancellationToken);
}
