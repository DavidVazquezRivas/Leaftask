using Microsoft.EntityFrameworkCore;
using Modules.WorkItems.DrivenInfrastructure.Persistence;

namespace Modules.WorkItems.DrivenInfrastructure.Persistence.Seeding;

public static class FieldReadModelBackfillSeeder
{
    public static async Task SeedAsync(WorkItemsDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.ExecuteSqlRawAsync("""
            INSERT INTO workitem.field_read_models ("Id", name, is_optional, field_type_read_model_id)
            SELECT f."Id", f.name, f.is_optional, f.field_type_id
            FROM project.fields f
            WHERE NOT EXISTS (
                SELECT 1 FROM workitem.field_read_models frm WHERE frm."Id" = f."Id"
            )
            """, cancellationToken);

        await dbContext.Database.ExecuteSqlRawAsync("""
            INSERT INTO workitem.field_read_model_work_item_types (field_read_model_id, work_item_type_id)
            SELECT fwt."FieldId", fwt."AppliesToId"
            FROM project.field_workitem_type_read_models fwt
            WHERE NOT EXISTS (
                SELECT 1 FROM workitem.field_read_model_work_item_types wfwt
                WHERE wfwt.field_read_model_id = fwt."FieldId"
                AND wfwt.work_item_type_id = fwt."AppliesToId"
            )
            """, cancellationToken);
    }
}
