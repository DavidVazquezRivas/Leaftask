using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Agents.DrivenInfrastructure.Migrations;

/// <inheritdoc />
public partial class AddHumanInTheLoop : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "agent_execution_queues",
            schema: "agent");

        migrationBuilder.CreateTable(
            name: "agent_executions",
            schema: "agent",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                payload = table.Column<string>(type: "text", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                agent_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_agent_executions", x => x.id);
                table.ForeignKey(
                    name: "FK_agent_executions_agents_agent_id",
                    column: x => x.agent_id,
                    principalSchema: "agent",
                    principalTable: "agents",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "agent_execution_messages",
            schema: "agent",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                execution_id = table.Column<Guid>(type: "uuid", nullable: false),
                role = table.Column<int>(type: "integer", nullable: false),
                content = table.Column<string>(type: "text", nullable: false),
                tool_calls = table.Column<string>(type: "text", nullable: true),
                sequence_number = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_agent_execution_messages", x => x.id));

        migrationBuilder.CreateTable(
            name: "agent_execution_pending_events",
            schema: "agent",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                execution_id = table.Column<Guid>(type: "uuid", nullable: false),
                event_type = table.Column<string>(type: "text", nullable: false),
                correlation_id = table.Column<string>(type: "text", nullable: false),
                is_resolved = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_agent_execution_pending_events", x => x.id);
                table.ForeignKey(
                    name: "FK_agent_execution_pending_events_agent_executions_execution_id",
                    column: x => x.execution_id,
                    principalSchema: "agent",
                    principalTable: "agent_executions",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_agent_execution_messages_execution_id_sequence_number",
            schema: "agent",
            table: "agent_execution_messages",
            columns: ["execution_id", "sequence_number"],
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_agent_execution_pending_events_execution_id",
            schema: "agent",
            table: "agent_execution_pending_events",
            column: "execution_id");

        migrationBuilder.CreateIndex(
            name: "IX_agent_executions_agent_id",
            schema: "agent",
            table: "agent_executions",
            column: "agent_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "agent_execution_messages", schema: "agent");
        migrationBuilder.DropTable(name: "agent_execution_pending_events", schema: "agent");
        migrationBuilder.DropTable(name: "agent_executions", schema: "agent");

        migrationBuilder.CreateTable(
            name: "agent_execution_queues",
            schema: "agent",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                agent_id = table.Column<Guid>(type: "uuid", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                payload = table.Column<string>(type: "text", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_agent_execution_queues", x => x.id);
                table.ForeignKey(
                    name: "FK_agent_execution_queues_agents_agent_id",
                    column: x => x.agent_id,
                    principalSchema: "agent",
                    principalTable: "agents",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_agent_execution_queues_agent_id",
            schema: "agent",
            table: "agent_execution_queues",
            column: "agent_id");
    }
}
