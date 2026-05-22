using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Projects.DrivenInfrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AbbreviationUniquePerOwner : Migration
{
    private static readonly string[] Columns = ["owner_id", "abbreviation"];

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_projects_abbreviation",
            schema: "project",
            table: "projects");

        migrationBuilder.CreateIndex(
            name: "IX_projects_owner_id_abbreviation",
            schema: "project",
            table: "projects",
            columns: Columns,
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_projects_owner_id_abbreviation",
            schema: "project",
            table: "projects");

        migrationBuilder.CreateIndex(
            name: "IX_projects_abbreviation",
            schema: "project",
            table: "projects",
            column: "abbreviation",
            unique: true);
    }
}
