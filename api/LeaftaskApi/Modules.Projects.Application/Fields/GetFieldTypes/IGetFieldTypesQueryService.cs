namespace Modules.Projects.Application.Fields.GetFieldTypes;

public interface IGetFieldTypesQueryService
{
    Task<IReadOnlyList<FieldTypeDto>> GetFieldTypesAsync(CancellationToken cancellationToken = default);
}
