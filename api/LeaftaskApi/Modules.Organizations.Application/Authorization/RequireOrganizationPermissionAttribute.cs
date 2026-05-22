namespace Modules.Organizations.Application.Authorization;

[AttributeUsage(AttributeTargets.Class)]
public sealed class RequireOrganizationPermissionAttribute(string permissionName) : Attribute
{
    public string PermissionName { get; } = permissionName;
}
