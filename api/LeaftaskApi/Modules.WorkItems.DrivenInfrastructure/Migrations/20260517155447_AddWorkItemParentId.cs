using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.WorkItems.DrivenInfrastructure.Migrations;

/// <inheritdoc />
public partial class AddWorkItemParentId : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "parent_id",
            schema: "workitem",
            table: "work_items",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_work_items_parent_id",
            schema: "workitem",
            table: "work_items",
            column: "parent_id");

        migrationBuilder.AddForeignKey(
            name: "FK_work_items_work_items_parent_id",
            schema: "workitem",
            table: "work_items",
            column: "parent_id",
            principalSchema: "workitem",
            principalTable: "work_items",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_work_items_work_items_parent_id",
            schema: "workitem",
            table: "work_items");

        migrationBuilder.DropIndex(
            name: "IX_work_items_parent_id",
            schema: "workitem",
            table: "work_items");

        migrationBuilder.DropColumn(
            name: "parent_id",
            schema: "workitem",
            table: "work_items");
    }
}

