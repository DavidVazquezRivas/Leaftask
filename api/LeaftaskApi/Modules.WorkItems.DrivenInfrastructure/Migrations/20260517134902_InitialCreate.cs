using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.WorkItems.DrivenInfrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    private static readonly string[] FieldValueColumns = ["field_read_model_id", "work_item_id"];

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "workitem");

        migrationBuilder.CreateTable(
            name: "field_type_read_models",
            schema: "workitem",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_field_type_read_models", x => x.Id));

        migrationBuilder.CreateTable(
            name: "InboxMessages",
            schema: "workitem",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<string>(type: "text", nullable: false),
                Content = table.Column<string>(type: "text", nullable: false),
                ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_InboxMessages", x => x.Id));

        migrationBuilder.CreateTable(
            name: "OutboxMessages",
            schema: "workitem",
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
            name: "project_read_models",
            schema: "workitem",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                abbreviation = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_project_read_models", x => x.Id));

        migrationBuilder.CreateTable(
            name: "user_read_models",
            schema: "workitem",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_user_read_models", x => x.Id));

        migrationBuilder.CreateTable(
            name: "work_item_statuses",
            schema: "workitem",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_work_item_statuses", x => x.Id));

        migrationBuilder.CreateTable(
            name: "work_item_types",
            schema: "workitem",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_work_item_types", x => x.Id));

        migrationBuilder.CreateTable(
            name: "field_read_models",
            schema: "workitem",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                is_optional = table.Column<bool>(type: "boolean", nullable: false),
                field_type_read_model_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_field_read_models", x => x.Id);
                table.ForeignKey(
                    name: "FK_field_read_models_field_type_read_models_field_type_read_mo~",
                    column: x => x.field_type_read_model_id,
                    principalSchema: "workitem",
                    principalTable: "field_type_read_models",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "work_items",
            schema: "workitem",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                code = table.Column<int>(type: "integer", nullable: false),
                title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                description = table.Column<string>(type: "text", nullable: false),
                progress = table.Column<int>(type: "integer", nullable: false),
                limit_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                project_read_model_id = table.Column<Guid>(type: "uuid", nullable: false),
                status_id = table.Column<Guid>(type: "uuid", nullable: false),
                type_id = table.Column<Guid>(type: "uuid", nullable: false),
                assignee_id = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_work_items", x => x.Id);
                table.ForeignKey(
                    name: "FK_work_items_project_read_models_project_read_model_id",
                    column: x => x.project_read_model_id,
                    principalSchema: "workitem",
                    principalTable: "project_read_models",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_work_items_user_read_models_assignee_id",
                    column: x => x.assignee_id,
                    principalSchema: "workitem",
                    principalTable: "user_read_models",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_work_items_work_item_statuses_status_id",
                    column: x => x.status_id,
                    principalSchema: "workitem",
                    principalTable: "work_item_statuses",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_work_items_work_item_types_type_id",
                    column: x => x.type_id,
                    principalSchema: "workitem",
                    principalTable: "work_item_types",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "activity_logs",
            schema: "workitem",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                field_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                old_value = table.Column<string>(type: "text", nullable: false),
                new_value = table.Column<string>(type: "text", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                work_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_read_model_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_activity_logs", x => x.Id);
                table.ForeignKey(
                    name: "FK_activity_logs_user_read_models_user_read_model_id",
                    column: x => x.user_read_model_id,
                    principalSchema: "workitem",
                    principalTable: "user_read_models",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_activity_logs_work_items_work_item_id",
                    column: x => x.work_item_id,
                    principalSchema: "workitem",
                    principalTable: "work_items",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "field_values",
            schema: "workitem",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                field_read_model_id = table.Column<Guid>(type: "uuid", nullable: false),
                work_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                value = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_field_values", x => x.Id);
                table.ForeignKey(
                    name: "FK_field_values_field_read_models_field_read_model_id",
                    column: x => x.field_read_model_id,
                    principalSchema: "workitem",
                    principalTable: "field_read_models",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_field_values_work_items_work_item_id",
                    column: x => x.work_item_id,
                    principalSchema: "workitem",
                    principalTable: "work_items",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "work_item_comments",
            schema: "workitem",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                content = table.Column<string>(type: "text", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                work_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_read_model_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_work_item_comments", x => x.Id);
                table.ForeignKey(
                    name: "FK_work_item_comments_user_read_models_user_read_model_id",
                    column: x => x.user_read_model_id,
                    principalSchema: "workitem",
                    principalTable: "user_read_models",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_work_item_comments_work_items_work_item_id",
                    column: x => x.work_item_id,
                    principalSchema: "workitem",
                    principalTable: "work_items",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "work_logs",
            schema: "workitem",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                hours = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                comment = table.Column<string>(type: "text", nullable: false),
                work_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_read_model_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_work_logs", x => x.Id);
                table.ForeignKey(
                    name: "FK_work_logs_user_read_models_user_read_model_id",
                    column: x => x.user_read_model_id,
                    principalSchema: "workitem",
                    principalTable: "user_read_models",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_work_logs_work_items_work_item_id",
                    column: x => x.work_item_id,
                    principalSchema: "workitem",
                    principalTable: "work_items",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "attachments",
            schema: "workitem",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                file_url = table.Column<string>(type: "text", nullable: false),
                uploaded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                work_item_id = table.Column<Guid>(type: "uuid", nullable: false),
                user_read_model_id = table.Column<Guid>(type: "uuid", nullable: false),
                comment_id = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_attachments", x => x.Id);
                table.ForeignKey(
                    name: "FK_attachments_user_read_models_user_read_model_id",
                    column: x => x.user_read_model_id,
                    principalSchema: "workitem",
                    principalTable: "user_read_models",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_attachments_work_item_comments_comment_id",
                    column: x => x.comment_id,
                    principalSchema: "workitem",
                    principalTable: "work_item_comments",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.SetNull);
                table.ForeignKey(
                    name: "FK_attachments_work_items_work_item_id",
                    column: x => x.work_item_id,
                    principalSchema: "workitem",
                    principalTable: "work_items",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_activity_logs_user_read_model_id",
            schema: "workitem",
            table: "activity_logs",
            column: "user_read_model_id");

        migrationBuilder.CreateIndex(
            name: "IX_activity_logs_work_item_id",
            schema: "workitem",
            table: "activity_logs",
            column: "work_item_id");

        migrationBuilder.CreateIndex(
            name: "IX_attachments_comment_id",
            schema: "workitem",
            table: "attachments",
            column: "comment_id");

        migrationBuilder.CreateIndex(
            name: "IX_attachments_user_read_model_id",
            schema: "workitem",
            table: "attachments",
            column: "user_read_model_id");

        migrationBuilder.CreateIndex(
            name: "IX_attachments_work_item_id",
            schema: "workitem",
            table: "attachments",
            column: "work_item_id");

        migrationBuilder.CreateIndex(
            name: "IX_field_read_models_field_type_read_model_id",
            schema: "workitem",
            table: "field_read_models",
            column: "field_type_read_model_id");

        migrationBuilder.CreateIndex(
            name: "IX_field_values_field_read_model_id_work_item_id",
            schema: "workitem",
            table: "field_values",
            columns: FieldValueColumns,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_field_values_work_item_id",
            schema: "workitem",
            table: "field_values",
            column: "work_item_id");

        migrationBuilder.CreateIndex(
            name: "IX_work_item_comments_user_read_model_id",
            schema: "workitem",
            table: "work_item_comments",
            column: "user_read_model_id");

        migrationBuilder.CreateIndex(
            name: "IX_work_item_comments_work_item_id",
            schema: "workitem",
            table: "work_item_comments",
            column: "work_item_id");

        migrationBuilder.CreateIndex(
            name: "IX_work_items_assignee_id",
            schema: "workitem",
            table: "work_items",
            column: "assignee_id");

        migrationBuilder.CreateIndex(
            name: "IX_work_items_project_read_model_id",
            schema: "workitem",
            table: "work_items",
            column: "project_read_model_id");

        migrationBuilder.CreateIndex(
            name: "IX_work_items_status_id",
            schema: "workitem",
            table: "work_items",
            column: "status_id");

        migrationBuilder.CreateIndex(
            name: "IX_work_items_type_id",
            schema: "workitem",
            table: "work_items",
            column: "type_id");

        migrationBuilder.CreateIndex(
            name: "IX_work_logs_user_read_model_id",
            schema: "workitem",
            table: "work_logs",
            column: "user_read_model_id");

        migrationBuilder.CreateIndex(
            name: "IX_work_logs_work_item_id",
            schema: "workitem",
            table: "work_logs",
            column: "work_item_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "activity_logs",
            schema: "workitem");

        migrationBuilder.DropTable(
            name: "attachments",
            schema: "workitem");

        migrationBuilder.DropTable(
            name: "field_values",
            schema: "workitem");

        migrationBuilder.DropTable(
            name: "InboxMessages",
            schema: "workitem");

        migrationBuilder.DropTable(
            name: "OutboxMessages",
            schema: "workitem");

        migrationBuilder.DropTable(
            name: "work_logs",
            schema: "workitem");

        migrationBuilder.DropTable(
            name: "work_item_comments",
            schema: "workitem");

        migrationBuilder.DropTable(
            name: "field_read_models",
            schema: "workitem");

        migrationBuilder.DropTable(
            name: "work_items",
            schema: "workitem");

        migrationBuilder.DropTable(
            name: "field_type_read_models",
            schema: "workitem");

        migrationBuilder.DropTable(
            name: "project_read_models",
            schema: "workitem");

        migrationBuilder.DropTable(
            name: "user_read_models",
            schema: "workitem");

        migrationBuilder.DropTable(
            name: "work_item_statuses",
            schema: "workitem");

        migrationBuilder.DropTable(
            name: "work_item_types",
            schema: "workitem");
    }
}
