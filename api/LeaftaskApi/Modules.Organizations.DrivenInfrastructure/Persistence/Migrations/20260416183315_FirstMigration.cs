using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Organizations.DrivenInfrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class FirstMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "organization");

        migrationBuilder.CreateTable(
            name: "organizations",
            schema: "organization",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                website = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_organizations", x => x.Id));

        migrationBuilder.CreateTable(
            name: "OutboxMessages",
            schema: "organization",
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
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "organizations",
            schema: "organization");

        migrationBuilder.DropTable(
            name: "OutboxMessages",
            schema: "organization");
    }
}

