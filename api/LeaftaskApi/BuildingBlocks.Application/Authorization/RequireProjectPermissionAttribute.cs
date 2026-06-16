namespace BuildingBlocks.Application.Authorization;

[AttributeUsage(AttributeTargets.Class)]
public sealed class RequireProjectPermissionAttribute(string permissionName) : Attribute
{
    public string PermissionName { get; } = permissionName;
}
