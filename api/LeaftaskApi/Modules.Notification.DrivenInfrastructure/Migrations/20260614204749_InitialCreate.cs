using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Notification.DrivenInfrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "notification");

        migrationBuilder.CreateTable(
            name: "actions",
            schema: "notification",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_actions", x => x.Id));

        migrationBuilder.CreateTable(
            name: "InboxMessages",
            schema: "notification",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<string>(type: "text", nullable: false),
                Content = table.Column<string>(type: "text", nullable: false),
                ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_InboxMessages", x => x.Id));

        migrationBuilder.CreateTable(
            name: "notifications",
            schema: "notification",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                type = table.Column<int>(type: "integer", nullable: false),
                context_id = table.Column<Guid>(type: "uuid", nullable: false),
                target_id = table.Column<Guid>(type: "uuid", nullable: false),
                recipient_id = table.Column<Guid>(type: "uuid", nullable: false),
                actor_id = table.Column<Guid>(type: "uuid", nullable: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_notifications", x => x.Id));

        migrationBuilder.CreateTable(
            name: "organization_permission_read_models",
            schema: "notification",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                permission_name = table.Column<string>(type: "text", nullable: false),
                level = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_organization_permission_read_models", x => x.Id));

        migrationBuilder.CreateTable(
            name: "OutboxMessages",
            schema: "notification",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<string>(type: "text", nullable: false),
                Content = table.Column<string>(type: "text", nullable: false),
                OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Error = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_OutboxMessages", x => x.Id));

        migrationBuilder.CreateTable(
            name: "project_permission_read_models",
            schema: "notification",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_project_permission_read_models", x => x.Id));

        migrationBuilder.CreateTable(
            name: "user_read_models",
            schema: "notification",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_user_read_models", x => x.Id));

        migrationBuilder.CreateTable(
            name: "approval_requests",
            schema: "notification",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                context_type = table.Column<int>(type: "integer", nullable: false),
                context_id = table.Column<Guid>(type: "uuid", nullable: false),
                target_id = table.Column<Guid>(type: "uuid", nullable: false),
                permission_name = table.Column<string>(type: "text", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                requester_id = table.Column<Guid>(type: "uuid", nullable: false),
                approver_rejecter_id = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_approval_requests", x => x.Id);
                table.ForeignKey(
                    name: "FK_approval_requests_user_read_models_approver_rejecter_id",
                    column: x => x.approver_rejecter_id,
                    principalSchema: "notification",
                    principalTable: "user_read_models",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_approval_requests_user_read_models_requester_id",
                    column: x => x.requester_id,
                    principalSchema: "notification",
                    principalTable: "user_read_models",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "request_comments",
            schema: "notification",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                request_id = table.Column<Guid>(type: "uuid", nullable: false),
                content = table.Column<string>(type: "text", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                created_by_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_request_comments", x => x.Id);
                table.ForeignKey(
                    name: "FK_request_comments_approval_requests_request_id",
                    column: x => x.request_id,
                    principalSchema: "notification",
                    principalTable: "approval_requests",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_request_comments_user_read_models_created_by_id",
                    column: x => x.created_by_id,
                    principalSchema: "notification",
                    principalTable: "user_read_models",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_approval_requests_approver_rejecter_id",
            schema: "notification",
            table: "approval_requests",
            column: "approver_rejecter_id");

        migrationBuilder.CreateIndex(
            name: "IX_approval_requests_requester_id",
            schema: "notification",
            table: "approval_requests",
            column: "requester_id");

        migrationBuilder.CreateIndex(
            name: "IX_request_comments_created_by_id",
            schema: "notification",
            table: "request_comments",
            column: "created_by_id");

        migrationBuilder.CreateIndex(
            name: "IX_request_comments_request_id",
            schema: "notification",
            table: "request_comments",
            column: "request_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "actions",
            schema: "notification");

        migrationBuilder.DropTable(
            name: "InboxMessages",
            schema: "notification");

        migrationBuilder.DropTable(
            name: "notifications",
            schema: "notification");

        migrationBuilder.DropTable(
            name: "organization_permission_read_models",
            schema: "notification");

        migrationBuilder.DropTable(
            name: "OutboxMessages",
            schema: "notification");

        migrationBuilder.DropTable(
            name: "project_permission_read_models",
            schema: "notification");

        migrationBuilder.DropTable(
            name: "request_comments",
            schema: "notification");

        migrationBuilder.DropTable(
            name: "approval_requests",
            schema: "notification");

        migrationBuilder.DropTable(
            name: "user_read_models",
            schema: "notification");
    }
}
