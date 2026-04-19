namespace Modules.Organizations.Application.Authorization;

public interface IOrganizationPermissionRequest
{
    Guid OrganizationId { get; }
}
