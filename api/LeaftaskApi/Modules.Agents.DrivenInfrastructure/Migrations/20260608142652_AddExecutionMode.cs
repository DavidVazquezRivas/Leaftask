using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Agents.DrivenInfrastructure.Migrations;

/// <inheritdoc />
public partial class AddExecutionMode : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) =>
        migrationBuilder.AddColumn<int>(
            name: "mode",
            schema: "agent",
            table: "agent_executions",
            type: "integer",
            nullable: false,
            defaultValue: 0);

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropColumn(
            name: "mode",
            schema: "agent",
            table: "agent_executions");
}
