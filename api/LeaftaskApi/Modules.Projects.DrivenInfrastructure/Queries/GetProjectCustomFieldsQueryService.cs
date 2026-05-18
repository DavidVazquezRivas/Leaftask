using Microsoft.EntityFrameworkCore;
using Modules.Projects.Application.Fields.GetProjectCustomFields;
using Modules.Projects.Domain.Entities.Field;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivenInfrastructure.Queries;

public sealed class GetProjectCustomFieldsQueryService(ProjectsDbContext dbContext)
    : IGetProjectCustomFieldsQueryService
{
    public async Task<IReadOnlyList<CustomFieldDto>> GetCustomFieldsAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        List<ProjectField> projectFields = await dbContext.ProjectFields
            .AsNoTracking()
            .Include(pf => pf.Field)
                .ThenInclude(f => f.FieldType)
            .Include(pf => pf.Field)
                .ThenInclude(f => f.AppliesTo)
            .Where(pf => EF.Property<Guid>(pf, "project_id") == projectId)
            .OrderBy(pf => pf.Name)
            .ToListAsync(cancellationToken);

        if (projectFields.Count == 0)
        {
            return [];
        }

        List<Guid> fieldIds = projectFields.Select(pf => pf.Field.Id).ToList();

        List<OptionRow> optionRows = await dbContext.Options
            .AsNoTracking()
            .Where(o => fieldIds.Contains(o.Field.Id))
            .OrderBy(o => o.Name)
            .Select(o => new OptionRow(EF.Property<Guid>(o, "field_id"), o.Id, o.Name))
            .ToListAsync(cancellationToken);

        ILookup<Guid, CustomFieldOptionDto> optionsByField = optionRows
            .ToLookup(
                o => o.FieldId,
                o => new CustomFieldOptionDto(o.OptionId, o.OptionName));

        return projectFields
            .Select(pf => new CustomFieldDto(
                pf.Id,
                pf.Name,
                pf.Field.FieldType.Id,
                optionsByField[pf.Field.Id].ToList(),
                !pf.Optional,
                pf.Field.AppliesTo
                    .Select(wt => new CustomFieldWorkItemTypeDto(wt.Id, wt.Name))
                    .ToList()))
            .ToList();
    }

    private sealed record OptionRow(Guid FieldId, Guid OptionId, string OptionName);
}
