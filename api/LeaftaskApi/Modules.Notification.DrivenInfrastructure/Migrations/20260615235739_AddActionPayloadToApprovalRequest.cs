using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Notification.DrivenInfrastructure.Migrations;

/// <inheritdoc />
public partial class AddActionPayloadToApprovalRequest : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "action_payload",
            schema: "notification",
            table: "approval_requests",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "action_type",
            schema: "notification",
            table: "approval_requests",
            type: "text",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "action_payload",
            schema: "notification",
            table: "approval_requests");

        migrationBuilder.DropColumn(
            name: "action_type",
            schema: "notification",
            table: "approval_requests");
    }
}
