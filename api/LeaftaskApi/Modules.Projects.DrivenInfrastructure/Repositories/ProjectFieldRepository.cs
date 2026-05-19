using Microsoft.EntityFrameworkCore;
using Modules.Projects.Domain.Entities.Field;
using Modules.Projects.Domain.Repositories;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivenInfrastructure.Repositories;

public sealed class ProjectFieldRepository(ProjectsDbContext dbContext) : IProjectFieldRepository
{
    public async Task<FieldType?> GetFieldTypeByIdAsync(Guid fieldTypeId, CancellationToken cancellationToken = default) =>
        await dbContext.FieldTypes
            .FirstOrDefaultAsync(ft => ft.Id == fieldTypeId, cancellationToken);

    public async Task<List<WorkItemTypeReadModel>> GetWorkItemTypesByIdsAsync(
        IReadOnlyList<Guid> ids,
        CancellationToken cancellationToken = default) =>
        await dbContext.WorkItemTypeReadModels
            .Where(wt => ids.Contains(wt.Id))
            .ToListAsync(cancellationToken);

    public async Task AddFieldAsync(Field field, CancellationToken cancellationToken = default) =>
        await dbContext.Fields.AddAsync(field, cancellationToken);

    public async Task AddOptionsAsync(IEnumerable<Option> options, CancellationToken cancellationToken = default) =>
        await dbContext.Options.AddRangeAsync(options, cancellationToken);

    public async Task<List<Option>> GetOptionsTrackedByFieldIdAsync(
        Guid fieldId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Options
            .Where(o => EF.Property<Guid>(o, "field_id") == fieldId)
            .ToListAsync(cancellationToken);

    public void RemoveOptions(IEnumerable<Option> options) =>
        dbContext.Options.RemoveRange(options);

    public async Task AddAsync(ProjectField field, CancellationToken cancellationToken = default) =>
        await dbContext.ProjectFields.AddAsync(field, cancellationToken);

    public async Task<ProjectField?> GetByIdTrackedAsync(
        Guid projectId,
        Guid fieldId,
        CancellationToken cancellationToken = default) =>
        await dbContext.ProjectFields
            .Include(pf => pf.Field)
                .ThenInclude(f => f.FieldType)
            .Include(pf => pf.Field)
                .ThenInclude(f => f.AppliesTo)
            .FirstOrDefaultAsync(
                pf => pf.Field.Id == fieldId && EF.Property<Guid>(pf, "project_id") == projectId,
                cancellationToken);

    public void Remove(ProjectField field) =>
        dbContext.ProjectFields.Remove(field);

    public void RemoveField(Field field) =>
        dbContext.Fields.Remove(field);

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.SaveChangesAsync(cancellationToken);
}
