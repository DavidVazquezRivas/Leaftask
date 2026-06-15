namespace Modules.Organizations.Domain.Events;

public sealed record OrganizationPermissionEntry(string PermissionName, int Level);
