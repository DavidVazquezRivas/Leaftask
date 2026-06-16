using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Agents.DrivenInfrastructure.Migrations;

/// <inheritdoc />
public partial class AddAgentProjectFields : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "description",
            schema: "agent",
            table: "agents",
            newName: "instructions");

        migrationBuilder.AddColumn<DateTime>(
            name: "created_at",
            schema: "agent",
            table: "agents",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<Guid>(
            name: "project_id",
            schema: "agent",
            table: "agents",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.AddColumn<Guid>(
            name: "template_id",
            schema: "agent",
            table: "agents",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "project_read_models",
            schema: "agent",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_project_read_models", x => x.id));

        migrationBuilder.CreateIndex(
            name: "IX_agents_project_id",
            schema: "agent",
            table: "agents",
            column: "project_id");

        migrationBuilder.AddForeignKey(
            name: "FK_agents_project_read_models_project_id",
            schema: "agent",
            table: "agents",
            column: "project_id",
            principalSchema: "agent",
            principalTable: "project_read_models",
            principalColumn: "id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_agents_project_read_models_project_id",
            schema: "agent",
            table: "agents");

        migrationBuilder.DropTable(
            name: "project_read_models",
            schema: "agent");

        migrationBuilder.DropIndex(
            name: "IX_agents_project_id",
            schema: "agent",
            table: "agents");

        migrationBuilder.DropColumn(
            name: "created_at",
            schema: "agent",
            table: "agents");

        migrationBuilder.DropColumn(
            name: "project_id",
            schema: "agent",
            table: "agents");

        migrationBuilder.DropColumn(
            name: "template_id",
            schema: "agent",
            table: "agents");

        migrationBuilder.RenameColumn(
            name: "instructions",
            schema: "agent",
            table: "agents",
            newName: "description");
    }
}
