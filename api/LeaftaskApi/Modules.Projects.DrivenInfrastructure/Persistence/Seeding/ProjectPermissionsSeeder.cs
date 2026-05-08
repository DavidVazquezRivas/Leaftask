using Microsoft.EntityFrameworkCore;
using Modules.Projects.Domain.Entities.Role;
using Modules.Projects.DrivenInfrastructure.Persistence;

namespace Modules.Projects.DrivenInfrastructure.Persistence.Seeding;

public static class ProjectPermissionsSeeder
{
    private static readonly Guid ConfigurationGroupId = new("a1b2c3d4-0001-0000-0000-000000000001");
    private static readonly Guid WorkItemsGroupId = new("a1b2c3d4-0002-0000-0000-000000000001");

    private static readonly ProjectPermissionGroup[] Groups =
    [
        new(ConfigurationGroupId, "Configuración"),
        new(WorkItemsGroupId, "Elementos de trabajo")
    ];

    private static readonly (Guid Id, string Name, string Description, Guid GroupId)[] PermissionData =
    [
        (new("b1c2d3e4-0001-0000-0000-000000000001"), "project.settings", "Configuración básica del proyecto", ConfigurationGroupId),
        (new("b1c2d3e4-0002-0000-0000-000000000001"), "project.custom-fields", "Configurar campos personalizados", ConfigurationGroupId),
        (new("b1c2d3e4-0003-0000-0000-000000000001"), "project.roles", "Configurar roles y permisos", ConfigurationGroupId),
        (new("b1c2d3e4-0004-0000-0000-000000000001"), "project.invite-members", "Invitar miembros", ConfigurationGroupId),
        (new("b1c2d3e4-0005-0000-0000-000000000001"), "project.member-roles", "Modificar Rol Miembros", ConfigurationGroupId),
        (new("b1c2d3e4-0006-0000-0000-000000000001"), "project.remove-members", "Expulsar miembros", ConfigurationGroupId),
        (new("b1c2d3e4-0007-0000-0000-000000000001"), "work-items.create", "Crear elementos de trabajo", WorkItemsGroupId),
        (new("b1c2d3e4-0008-0000-0000-000000000001"), "work-items.edit-definition", "Modificar definición (Título, Descripción, Fecha Límite, Padre, Prioridad)", WorkItemsGroupId),
        (new("b1c2d3e4-0009-0000-0000-000000000001"), "work-items.edit-estimation", "Modificar estimación (Estimación Horas)", WorkItemsGroupId),
        (new("b1c2d3e4-0010-0000-0000-000000000001"), "work-items.assign-self", "Modificar asignación (asignar a uno mismo)", WorkItemsGroupId),
        (new("b1c2d3e4-0011-0000-0000-000000000001"), "work-items.assign-any", "Modificar asignación (asignar a X)", WorkItemsGroupId),
        (new("b1c2d3e4-0012-0000-0000-000000000001"), "work-items.edit-own-progress", "Modificar avance y dedicación (asignadas)", WorkItemsGroupId),
        (new("b1c2d3e4-0013-0000-0000-000000000001"), "work-items.edit-any-progress", "Modificar avance y dedicación (ajenas)", WorkItemsGroupId),
        (new("b1c2d3e4-0014-0000-0000-000000000001"), "work-items.comment", "Añadir comentario", WorkItemsGroupId),
        (new("b1c2d3e4-0015-0000-0000-000000000001"), "agents.create", "Crear Agentes sobre el proyecto", WorkItemsGroupId)
    ];

    public static async Task SeedAsync(ProjectsDbContext dbContext, CancellationToken cancellationToken = default)
    {
        bool alreadySeeded = await dbContext.ProjectPermissions
            .AnyAsync(p => p.Name == "project.settings", cancellationToken);

        if (alreadySeeded)
        {
            return;
        }

        await dbContext.ProjectPermissions.ExecuteDeleteAsync(cancellationToken);
        await dbContext.ProjectPermissionGroups.ExecuteDeleteAsync(cancellationToken);

        await dbContext.ProjectPermissionGroups.AddRangeAsync(Groups, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        ProjectPermission[] permissions = PermissionData
            .Select(p =>
            {
                ProjectPermissionGroup group = Groups.First(g => g.Id == p.GroupId);
                return new ProjectPermission(p.Id, p.Name, p.Description, group);
            })
            .ToArray();

        await dbContext.ProjectPermissions.AddRangeAsync(permissions, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
