using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.WorkItems.DrivenInfrastructure.Migrations;

/// <inheritdoc />
public partial class AddFieldReadModelWorkItemTypes : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "field_read_model_work_item_types",
            schema: "workitem",
            columns: table => new
            {
                field_read_model_id = table.Column<Guid>(type: "uuid", nullable: false),
                work_item_type_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_field_read_model_work_item_types", x => new { x.field_read_model_id, x.work_item_type_id });
                table.ForeignKey(
                    name: "FK_field_read_model_work_item_types_field_read_models_field_re~",
                    column: x => x.field_read_model_id,
                    principalSchema: "workitem",
                    principalTable: "field_read_models",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_field_read_model_work_item_types_work_item_types_work_item_~",
                    column: x => x.work_item_type_id,
                    principalSchema: "workitem",
                    principalTable: "work_item_types",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_field_read_model_work_item_types_work_item_type_id",
            schema: "workitem",
            table: "field_read_model_work_item_types",
            column: "work_item_type_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "field_read_model_work_item_types",
            schema: "workitem");
    }
}
