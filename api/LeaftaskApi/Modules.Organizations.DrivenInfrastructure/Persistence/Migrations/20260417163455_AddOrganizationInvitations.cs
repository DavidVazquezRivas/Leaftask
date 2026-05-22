using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Organizations.DrivenInfrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddOrganizationInvitations : Migration
{
    private static readonly string[] OrganizationInvitationByStatusColumns = ["organization_id", "status"];
    private static readonly string[] OrganizationInvitationByUserColumns = ["organization_id", "user_id"];

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "organization_invitations",
            schema: "organization",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                invited_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                responded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                abandoned_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_organization_invitations", x => x.Id);
                table.ForeignKey(
                    name: "FK_organization_invitations_organizations_organization_id",
                    column: x => x.organization_id,
                    principalSchema: "organization",
                    principalTable: "organizations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_organization_invitations_organization_id_status",
            schema: "organization",
            table: "organization_invitations",
            columns: OrganizationInvitationByStatusColumns);

        migrationBuilder.CreateIndex(
            name: "IX_organization_invitations_organization_id_user_id",
            schema: "organization",
            table: "organization_invitations",
            columns: OrganizationInvitationByUserColumns,
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "organization_invitations",
            schema: "organization");
    }
}
