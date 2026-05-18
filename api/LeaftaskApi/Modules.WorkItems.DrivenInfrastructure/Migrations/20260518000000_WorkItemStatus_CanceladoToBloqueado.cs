using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.WorkItems.DrivenInfrastructure.Migrations;

/// <inheritdoc />
public partial class WorkItemStatus_CanceladoToBloqueado : Migration
{
    private static readonly Guid CanceladoId = new("a1000000-0005-0000-0000-000000000001");

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            $"UPDATE workitem.work_item_statuses SET name = 'Bloqueado' WHERE \"Id\" = '{CanceladoId}'");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            $"UPDATE workitem.work_item_statuses SET name = 'Cancelado' WHERE \"Id\" = '{CanceladoId}'");
    }
}
