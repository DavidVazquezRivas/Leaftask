using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Projects.DrivenInfrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddIsOwnerRoleToProjectRole : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "is_owner_role",
            schema: "project",
            table: "project_roles",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "is_owner_role",
            schema: "project",
            table: "project_roles");
    }
}
