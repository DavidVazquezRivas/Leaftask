using Microsoft.EntityFrameworkCore;
using Modules.Projects.Application.Fields.GetFieldTypes;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivenInfrastructure.Queries;

public sealed class GetFieldTypesQueryService(ProjectsDbContext dbContext)
    : IGetFieldTypesQueryService
{
    public async Task<IReadOnlyList<FieldTypeDto>> GetFieldTypesAsync(CancellationToken cancellationToken = default) =>
        await dbContext.FieldTypes
            .AsNoTracking()
            .OrderBy(ft => ft.Name)
            .Select(ft => new FieldTypeDto(ft.Id, ft.Name, ft.Description))
            .ToListAsync(cancellationToken);
}
