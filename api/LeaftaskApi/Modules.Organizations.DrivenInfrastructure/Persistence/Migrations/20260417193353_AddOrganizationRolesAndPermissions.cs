using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Organizations.DrivenInfrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddOrganizationRolesAndPermissions : Migration
{
    private static readonly string[] columns = ["organization_role_id", "organization_permission_id"];
    private static readonly string[] columnsArray = ["organization_id", "name"];

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "organization_role_id",
            schema: "organization",
            table: "organization_invitations",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "organization_permissions",
            schema: "organization",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false)
            },
            constraints: table =>
                table.PrimaryKey("PK_organization_permissions", x => x.Id));

        migrationBuilder.CreateTable(
            name: "organization_roles",
            schema: "organization",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                organization_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_organization_roles", x => x.Id);
                table.ForeignKey(
                    name: "FK_organization_roles_organizations_organization_id",
                    column: x => x.organization_id,
                    principalSchema: "organization",
                    principalTable: "organizations",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "organization_role_permissions",
            schema: "organization",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                level = table.Column<int>(type: "integer", nullable: false),
                organization_role_id = table.Column<Guid>(type: "uuid", nullable: false),
                organization_permission_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_organization_role_permissions", x => x.Id);
                table.ForeignKey(
                    name: "FK_organization_role_permissions_organization_permissions_orga~",
                    column: x => x.organization_permission_id,
                    principalSchema: "organization",
                    principalTable: "organization_permissions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_organization_role_permissions_organization_roles_organizati~",
                    column: x => x.organization_role_id,
                    principalSchema: "organization",
                    principalTable: "organization_roles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_organization_invitations_organization_role_id",
            schema: "organization",
            table: "organization_invitations",
            column: "organization_role_id");

        migrationBuilder.CreateIndex(
            name: "IX_organization_permissions_name",
            schema: "organization",
            table: "organization_permissions",
            column: "name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_organization_role_permissions_organization_permission_id",
            schema: "organization",
            table: "organization_role_permissions",
            column: "organization_permission_id");

        migrationBuilder.CreateIndex(
            name: "IX_organization_role_permissions_organization_role_id_organiza~",
            schema: "organization",
            table: "organization_role_permissions",
            columns: columns,
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_organization_roles_organization_id_name",
            schema: "organization",
            table: "organization_roles",
            columns: columnsArray,
            unique: true);

        migrationBuilder.Sql("""
            INSERT INTO organization.organization_roles ("Id", name, organization_id)
            SELECT o."Id", 'Owner', o."Id"
            FROM organization.organizations o
            WHERE NOT EXISTS (
                SELECT 1
                FROM organization.organization_roles r
                WHERE r.organization_id = o."Id"
            );
            """);

        migrationBuilder.Sql("""
            UPDATE organization.organization_invitations i
            SET organization_role_id = r."Id"
            FROM organization.organization_roles r
            WHERE i.organization_id = r.organization_id
              AND i.organization_role_id IS NULL;
            """);

        migrationBuilder.AlterColumn<Guid>(
            name: "organization_role_id",
            schema: "organization",
            table: "organization_invitations",
            type: "uuid",
            nullable: false,
            oldClrType: typeof(Guid),
            oldType: "uuid",
            oldNullable: true);

        migrationBuilder.AddForeignKey(
            name: "FK_organization_invitations_organization_roles_organization_ro~",
            schema: "organization",
            table: "organization_invitations",
            column: "organization_role_id",
            principalSchema: "organization",
            principalTable: "organization_roles",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_organization_invitations_organization_roles_organization_ro~",
            schema: "organization",
            table: "organization_invitations");

        migrationBuilder.DropTable(
            name: "organization_role_permissions",
            schema: "organization");

        migrationBuilder.DropTable(
            name: "organization_permissions",
            schema: "organization");

        migrationBuilder.DropTable(
            name: "organization_roles",
            schema: "organization");

        migrationBuilder.DropIndex(
            name: "IX_organization_invitations_organization_role_id",
            schema: "organization",
            table: "organization_invitations");

        migrationBuilder.DropColumn(
            name: "organization_role_id",
            schema: "organization",
            table: "organization_invitations");
    }
}

