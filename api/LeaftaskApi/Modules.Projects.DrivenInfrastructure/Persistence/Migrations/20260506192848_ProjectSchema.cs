using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Projects.DrivenInfrastructure.Persistence.Migrations;
/// <inheritdoc />
public partial class ProjectSchema : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "project");

        migrationBuilder.CreateTable(
            name: "field_types",
            schema: "project",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
            },
            constraints: table =>
                table.PrimaryKey("PK_field_types", x => x.Id));

        migrationBuilder.CreateTable(
            name: "InboxMessages",
            schema: "project",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<string>(type: "text", nullable: false),
                Content = table.Column<string>(type: "text", nullable: false),
                ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
                table.PrimaryKey("PK_InboxMessages", x => x.Id));

        migrationBuilder.CreateTable(
            name: "organizations_read_models",
            schema: "project",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
                table.PrimaryKey("PK_organizations_read_models", x => x.Id));

        migrationBuilder.CreateTable(
            name: "OutboxMessages",
            schema: "project",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<string>(type: "text", nullable: false),
                Content = table.Column<string>(type: "text", nullable: false),
                OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                Error = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
                table.PrimaryKey("PK_OutboxMessages", x => x.Id));

        migrationBuilder.CreateTable(
            name: "project_permission_groups",
            schema: "project",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table =>
                table.PrimaryKey("PK_project_permission_groups", x => x.Id));

        migrationBuilder.CreateTable(
            name: "projects",
            schema: "project",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                abbreviation = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                privacy = table.Column<int>(type: "integer", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                owner_id = table.Column<Guid>(type: "uuid", nullable: false),
                owner_type = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
                table.PrimaryKey("PK_projects", x => x.Id));

        migrationBuilder.CreateTable(
            name: "users_read_models",
            schema: "project",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false)
            },
            constraints: table =>
                table.PrimaryKey("PK_users_read_models", x => x.Id));

        migrationBuilder.CreateTable(
            name: "fields",
            schema: "project",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                is_optional = table.Column<bool>(type: "boolean", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                field_type_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_fields", x => x.Id);
                table.ForeignKey(
                    name: "FK_fields_field_types_field_type_id",
                    column: x => x.field_type_id,
                    principalSchema: "project",
                    principalTable: "field_types",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "project_permissions",
            schema: "project",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                permission_group_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_project_permissions", x => x.Id);
                table.ForeignKey(
                    name: "FK_project_permissions_project_permission_groups_permission_gr~",
                    column: x => x.permission_group_id,
                    principalSchema: "project",
                    principalTable: "project_permission_groups",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "project_roles",
            schema: "project",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                project_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_project_roles", x => x.Id);
                table.ForeignKey(
                    name: "FK_project_roles_projects_project_id",
                    column: x => x.project_id,
                    principalSchema: "project",
                    principalTable: "projects",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "options",
            schema: "project",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                field_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_options", x => x.Id);
                table.ForeignKey(
                    name: "FK_options_fields_field_id",
                    column: x => x.field_id,
                    principalSchema: "project",
                    principalTable: "fields",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "project_fields",
            schema: "project",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                @default = table.Column<bool>(name: "default", type: "boolean", nullable: false),
                optional = table.Column<bool>(type: "boolean", nullable: false),
                field_id = table.Column<Guid>(type: "uuid", nullable: false),
                project_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_project_fields", x => x.Id);
                table.ForeignKey(
                    name: "FK_project_fields_fields_field_id",
                    column: x => x.field_id,
                    principalSchema: "project",
                    principalTable: "fields",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_project_fields_projects_project_id",
                    column: x => x.project_id,
                    principalSchema: "project",
                    principalTable: "projects",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "project_role_permissions",
            schema: "project",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                permission_level = table.Column<int>(type: "integer", nullable: false),
                project_permission_id = table.Column<Guid>(type: "uuid", nullable: false),
                project_role_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_project_role_permissions", x => x.Id);
                table.ForeignKey(
                    name: "FK_project_role_permissions_project_permissions_project_permis~",
                    column: x => x.project_permission_id,
                    principalSchema: "project",
                    principalTable: "project_permissions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_project_role_permissions_project_roles_project_role_id",
                    column: x => x.project_role_id,
                    principalSchema: "project",
                    principalTable: "project_roles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_field_types_name",
            schema: "project",
            table: "field_types",
            column: "name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_fields_field_type_id",
            schema: "project",
            table: "fields",
            column: "field_type_id");

        migrationBuilder.CreateIndex(
            name: "IX_options_field_id",
            schema: "project",
            table: "options",
            column: "field_id");

        migrationBuilder.CreateIndex(
            name: "IX_project_fields_field_id",
            schema: "project",
            table: "project_fields",
            column: "field_id");

        migrationBuilder.CreateIndex(
            name: "IX_project_fields_project_id",
            schema: "project",
            table: "project_fields",
            column: "project_id");

        migrationBuilder.CreateIndex(
            name: "IX_project_permission_groups_name",
            schema: "project",
            table: "project_permission_groups",
            column: "name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_project_permissions_name",
            schema: "project",
            table: "project_permissions",
            column: "name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_project_permissions_permission_group_id",
            schema: "project",
            table: "project_permissions",
            column: "permission_group_id");

        migrationBuilder.CreateIndex(
            name: "IX_project_role_permissions_project_permission_id",
            schema: "project",
            table: "project_role_permissions",
            column: "project_permission_id");

        migrationBuilder.CreateIndex(
            name: "IX_project_role_permissions_project_role_id_project_permission~",
            schema: "project",
            table: "project_role_permissions",
            columns: ["project_role_id", "project_permission_id"],
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_project_roles_project_id_name",
            schema: "project",
            table: "project_roles",
            columns: ["project_id", "name"],
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_projects_abbreviation",
            schema: "project",
            table: "projects",
            column: "abbreviation",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_projects_owner_id_owner_type",
            schema: "project",
            table: "projects",
            columns: ["owner_id", "owner_type"]);

        migrationBuilder.CreateIndex(
            name: "IX_users_read_models_email",
            schema: "project",
            table: "users_read_models",
            column: "email",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "InboxMessages",
            schema: "project");

        migrationBuilder.DropTable(
            name: "options",
            schema: "project");

        migrationBuilder.DropTable(
            name: "organizations_read_models",
            schema: "project");

        migrationBuilder.DropTable(
            name: "OutboxMessages",
            schema: "project");

        migrationBuilder.DropTable(
            name: "project_fields",
            schema: "project");

        migrationBuilder.DropTable(
            name: "project_role_permissions",
            schema: "project");

        migrationBuilder.DropTable(
            name: "users_read_models",
            schema: "project");

        migrationBuilder.DropTable(
            name: "fields",
            schema: "project");

        migrationBuilder.DropTable(
            name: "project_permissions",
            schema: "project");

        migrationBuilder.DropTable(
            name: "project_roles",
            schema: "project");

        migrationBuilder.DropTable(
            name: "field_types",
            schema: "project");

        migrationBuilder.DropTable(
            name: "project_permission_groups",
            schema: "project");

        migrationBuilder.DropTable(
            name: "projects",
            schema: "project");
    }
}
