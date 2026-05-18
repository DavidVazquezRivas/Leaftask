using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Projects.DrivenInfrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddWorkItemTypeReadModelToFields : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "workitem_type_read_models",
            schema: "project",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_workitem_type_read_models", x => x.Id));

        migrationBuilder.CreateTable(
            name: "field_workitem_type_read_models",
            schema: "project",
            columns: table => new
            {
                AppliesToId = table.Column<Guid>(type: "uuid", nullable: false),
                FieldId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_field_workitem_type_read_models", x => new { x.AppliesToId, x.FieldId });
                table.ForeignKey(
                    name: "FK_field_workitem_type_read_models_fields_FieldId",
                    column: x => x.FieldId,
                    principalSchema: "project",
                    principalTable: "fields",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_field_workitem_type_read_models_workitem_type_read_models_A~",
                    column: x => x.AppliesToId,
                    principalSchema: "project",
                    principalTable: "workitem_type_read_models",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_field_workitem_type_read_models_FieldId",
            schema: "project",
            table: "field_workitem_type_read_models",
            column: "FieldId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "field_workitem_type_read_models",
            schema: "project");

        migrationBuilder.DropTable(
            name: "workitem_type_read_models",
            schema: "project");
    }
}
