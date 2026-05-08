namespace Modules.Projects.Application.Fields.GetProjectCustomFields;

public interface IGetProjectCustomFieldsQueryService
{
    Task<IReadOnlyList<CustomFieldDto>> GetCustomFieldsAsync(Guid projectId, CancellationToken cancellationToken = default);
}
