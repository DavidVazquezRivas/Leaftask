using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.WorkItems.DrivenInfrastructure.Migrations;

/// <inheritdoc />
public partial class WorkItem_AddEstimation : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "estimation",
            schema: "workitem",
            table: "work_items",
            type: "numeric(10,2)",
            precision: 10,
            scale: 2,
            nullable: false,
            defaultValue: 0m);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "estimation",
            schema: "workitem",
            table: "work_items");
    }
}
