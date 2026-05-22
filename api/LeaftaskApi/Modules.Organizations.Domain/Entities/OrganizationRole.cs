using BuildingBlocks.Domain.Result;
using Modules.Organizations.Domain.Errors;

namespace Modules.Organizations.Domain.Entities;

public sealed class OrganizationRole
{
    private readonly List<OrganizationRolePermission> _permissions = [];

    private OrganizationRole() { }

    public OrganizationRole(string name, Guid organizationId, IEnumerable<OrganizationPermission>? permissions = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        OrganizationId = organizationId;

        if (permissions is not null)
        {
            InitializePermissions(permissions);
        }
    }

    public Guid Id { get; }
    public string Name { get; private set; }
    public Guid OrganizationId { get; }
    public IReadOnlyCollection<OrganizationRolePermission> Permissions => _permissions.AsReadOnly();

    public Result Update(string name, IReadOnlyCollection<(Guid OrganizationPermissionId, PermissionLevel Level)>? permissions = null)
    {
        if (permissions is not null && permissions.Any(permission =>
                _permissions.All(rolePermission => rolePermission.OrganizationPermissionId != permission.OrganizationPermissionId)))
        {
            return Result.Failure(OrganizationErrors.OrganizationRolePermissionNotFound);
        }

        Name = name;

        if (permissions is not null)
        {
            foreach ((Guid organizationPermissionId, PermissionLevel level) in permissions)
            {
                _permissions.Single(rolePermission => rolePermission.OrganizationPermissionId == organizationPermissionId)
                    .UpdateLevel(level);
            }
        }

        return Result.Success();
    }

    public void InitializePermissions(IEnumerable<OrganizationPermission> permissions)
    {
        List<Guid> organizationPermissionIds = permissions.Select(permission => permission.Id).ToList();

        foreach (Guid organizationPermissionId in organizationPermissionIds)
        {
            if (_permissions.Any(rolePermission => rolePermission.OrganizationPermissionId == organizationPermissionId))
            {
                continue;
            }

            _permissions.Add(new OrganizationRolePermission(Id, organizationPermissionId, PermissionLevel.None));
        }
    }

    public Result SetPermissionLevel(Guid organizationPermissionId, PermissionLevel level)
    {
        OrganizationRolePermission? permission = _permissions.SingleOrDefault(rolePermission =>
            rolePermission.OrganizationPermissionId == organizationPermissionId);

        if (permission is null)
        {
            return Result.Failure(OrganizationErrors.OrganizationRolePermissionNotFound);
        }

        permission.UpdateLevel(level);
        return Result.Success();
    }

    public void RemovePermission(Guid organizationPermissionId) =>
        _permissions.RemoveAll(rolePermission => rolePermission.OrganizationPermissionId == organizationPermissionId);
}
