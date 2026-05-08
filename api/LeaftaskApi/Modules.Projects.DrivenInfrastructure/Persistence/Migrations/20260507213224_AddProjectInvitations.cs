using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Projects.DrivenInfrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddProjectInvitations : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "project_invitations",
            schema: "project",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                project_id = table.Column<Guid>(type: "uuid", nullable: false),
                invitee_id = table.Column<Guid>(type: "uuid", nullable: false),
                invitee_type = table.Column<int>(type: "integer", nullable: false),
                project_role_id = table.Column<Guid>(type: "uuid", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                invited_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                responded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                cancelled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_project_invitations", x => x.Id);
                table.ForeignKey(
                    name: "FK_project_invitations_project_roles_project_role_id",
                    column: x => x.project_role_id,
                    principalSchema: "project",
                    principalTable: "project_roles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_project_invitations_projects_project_id",
                    column: x => x.project_id,
                    principalSchema: "project",
                    principalTable: "projects",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

#pragma warning disable CA1861
        migrationBuilder.CreateIndex(
            name: "IX_project_invitations_project_id_invitee_id",
            schema: "project",
            table: "project_invitations",
            columns: new[] { "project_id", "invitee_id" },
            unique: true);
#pragma warning restore CA1861

        migrationBuilder.CreateIndex(
            name: "IX_project_invitations_project_role_id",
            schema: "project",
            table: "project_invitations",
            column: "project_role_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "project_invitations",
            schema: "project");
    }
}
