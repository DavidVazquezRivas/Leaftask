namespace Modules.Projects.Application.Authorization;

public interface IOrganizationPermissionRequest
{
    Guid? OrganizationId { get; }
}
