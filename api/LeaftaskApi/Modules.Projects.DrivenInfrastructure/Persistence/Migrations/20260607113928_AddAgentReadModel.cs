using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Projects.DrivenInfrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddAgentReadModel : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "agent_read_models",
            schema: "project",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_agent_read_models", x => x.Id));
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropTable(
            name: "agent_read_models",
            schema: "project");
}
