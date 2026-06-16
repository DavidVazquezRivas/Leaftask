using Microsoft.EntityFrameworkCore;

namespace Modules.Notification.DrivenInfrastructure.Persistence.Seeding;

public static class ProjectPermissionReadModelBackfillSeeder
{
    public static async Task SeedAsync(NotificationsDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.ExecuteSqlRawAsync("""
            INSERT INTO notification.project_permission_read_models ("Id", user_id, project_id, permission_name, level)
            SELECT
                gen_random_uuid(),
                pm.member_id,
                pm.project_id,
                pp.name,
                prp.permission_level
            FROM project.project_members pm
            INNER JOIN project.project_roles pr ON pr."Id" = pm.project_role_id
            INNER JOIN project.project_role_permissions prp ON prp.project_role_id = pr."Id"
            INNER JOIN project.project_permissions pp ON pp."Id" = prp.project_permission_id
            WHERE pm.member_type = 1
              AND NOT EXISTS (
                SELECT 1
                FROM notification.project_permission_read_models pprm
                WHERE pprm.user_id = pm.member_id
                  AND pprm.project_id = pm.project_id
                  AND pprm.permission_name = pp.name
              )
            """, cancellationToken);
    }
}
