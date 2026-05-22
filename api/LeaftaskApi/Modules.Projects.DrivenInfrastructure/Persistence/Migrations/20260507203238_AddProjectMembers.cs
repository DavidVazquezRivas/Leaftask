using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1861

namespace Modules.Projects.DrivenInfrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddProjectMembers : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "project_members",
            schema: "project",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                member_id = table.Column<Guid>(type: "uuid", nullable: false),
                member_type = table.Column<int>(type: "integer", nullable: false),
                project_role_id = table.Column<Guid>(type: "uuid", nullable: false),
                project_id = table.Column<Guid>(type: "uuid", nullable: false),
                joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_project_members", x => x.Id);
                table.ForeignKey(
                    name: "FK_project_members_project_roles_project_role_id",
                    column: x => x.project_role_id,
                    principalSchema: "project",
                    principalTable: "project_roles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_project_members_projects_project_id",
                    column: x => x.project_id,
                    principalSchema: "project",
                    principalTable: "projects",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_project_members_project_id_member_id",
            schema: "project",
            table: "project_members",
            columns: new[] { "project_id", "member_id" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_project_members_project_role_id",
            schema: "project",
            table: "project_members",
            column: "project_role_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "project_members",
            schema: "project");
    }
}
