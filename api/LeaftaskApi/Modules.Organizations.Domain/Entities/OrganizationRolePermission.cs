namespace Modules.Organizations.Domain.Entities;

public sealed class OrganizationRolePermission
{
    private OrganizationRolePermission() { }

    public OrganizationRolePermission(Guid organizationRoleId, Guid organizationPermissionId, PermissionLevel level)
    {
        Id = Guid.NewGuid();
        OrganizationRoleId = organizationRoleId;
        OrganizationPermissionId = organizationPermissionId;
        Level = level;
    }

    public Guid Id { get; }
    public PermissionLevel Level { get; private set; }
    public Guid OrganizationRoleId { get; }
    public Guid OrganizationPermissionId { get; }

    public void UpdateLevel(PermissionLevel level) => Level = level;
}
