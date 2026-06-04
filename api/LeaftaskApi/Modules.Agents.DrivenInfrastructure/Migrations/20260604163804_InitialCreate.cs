using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Agents.DrivenInfrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "agent");

        migrationBuilder.CreateTable(
            name: "agent_templates",
            schema: "agent",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "text", nullable: false),
                description = table.Column<string>(type: "text", nullable: false),
                instructions = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_agent_templates", x => x.id));

        migrationBuilder.CreateTable(
            name: "InboxMessages",
            schema: "agent",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<string>(type: "text", nullable: false),
                Content = table.Column<string>(type: "text", nullable: false),
                ReceivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_InboxMessages", x => x.Id));

        migrationBuilder.CreateTable(
            name: "model_providers",
            schema: "agent",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "text", nullable: false),
                token = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_model_providers", x => x.id));

        migrationBuilder.CreateTable(
            name: "OutboxMessages",
            schema: "agent",
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
            name: "models",
            schema: "agent",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "text", nullable: false),
                description = table.Column<string>(type: "text", nullable: false),
                cost = table.Column<double>(type: "double precision", nullable: false),
                provider_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_models", x => x.id);
                table.ForeignKey(
                    name: "FK_models_model_providers_provider_id",
                    column: x => x.provider_id,
                    principalSchema: "agent",
                    principalTable: "model_providers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "model_configs",
            schema: "agent",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                model_id = table.Column<Guid>(type: "uuid", nullable: false),
                temperature = table.Column<double>(type: "double precision", nullable: false),
                max_tokens = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_model_configs", x => x.id);
                table.ForeignKey(
                    name: "FK_model_configs_models_model_id",
                    column: x => x.model_id,
                    principalSchema: "agent",
                    principalTable: "models",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "agents",
            schema: "agent",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "text", nullable: false),
                description = table.Column<string>(type: "text", nullable: false),
                system_prompt = table.Column<string>(type: "text", nullable: false),
                model_config_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_agents", x => x.id);
                table.ForeignKey(
                    name: "FK_agents_model_configs_model_config_id",
                    column: x => x.model_config_id,
                    principalSchema: "agent",
                    principalTable: "model_configs",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "agent_event_triggers",
            schema: "agent",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_prompt = table.Column<string>(type: "text", nullable: false),
                @event = table.Column<string>(name: "event", type: "text", nullable: false),
                agent_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_agent_event_triggers", x => x.id);
                table.ForeignKey(
                    name: "FK_agent_event_triggers_agents_agent_id",
                    column: x => x.agent_id,
                    principalSchema: "agent",
                    principalTable: "agents",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "agent_execution_queues",
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
                table.PrimaryKey("PK_agent_execution_queues", x => x.id);
                table.ForeignKey(
                    name: "FK_agent_execution_queues_agents_agent_id",
                    column: x => x.agent_id,
                    principalSchema: "agent",
                    principalTable: "agents",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "agent_time_triggers",
            schema: "agent",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "text", nullable: false),
                cron_expression = table.Column<string>(type: "text", nullable: false),
                time_zone = table.Column<string>(type: "text", nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false),
                agent_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_agent_time_triggers", x => x.id);
                table.ForeignKey(
                    name: "FK_agent_time_triggers_agents_agent_id",
                    column: x => x.agent_id,
                    principalSchema: "agent",
                    principalTable: "agents",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_agent_event_triggers_agent_id",
            schema: "agent",
            table: "agent_event_triggers",
            column: "agent_id");

        migrationBuilder.CreateIndex(
            name: "IX_agent_execution_queues_agent_id",
            schema: "agent",
            table: "agent_execution_queues",
            column: "agent_id");

        migrationBuilder.CreateIndex(
            name: "IX_agent_time_triggers_agent_id",
            schema: "agent",
            table: "agent_time_triggers",
            column: "agent_id");

        migrationBuilder.CreateIndex(
            name: "IX_agents_model_config_id",
            schema: "agent",
            table: "agents",
            column: "model_config_id");

        migrationBuilder.CreateIndex(
            name: "IX_model_configs_model_id",
            schema: "agent",
            table: "model_configs",
            column: "model_id");

        migrationBuilder.CreateIndex(
            name: "IX_models_provider_id",
            schema: "agent",
            table: "models",
            column: "provider_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "agent_event_triggers",
            schema: "agent");

        migrationBuilder.DropTable(
            name: "agent_execution_queues",
            schema: "agent");

        migrationBuilder.DropTable(
            name: "agent_templates",
            schema: "agent");

        migrationBuilder.DropTable(
            name: "agent_time_triggers",
            schema: "agent");

        migrationBuilder.DropTable(
            name: "InboxMessages",
            schema: "agent");

        migrationBuilder.DropTable(
            name: "OutboxMessages",
            schema: "agent");

        migrationBuilder.DropTable(
            name: "agents",
            schema: "agent");

        migrationBuilder.DropTable(
            name: "model_configs",
            schema: "agent");

        migrationBuilder.DropTable(
            name: "models",
            schema: "agent");

        migrationBuilder.DropTable(
            name: "model_providers",
            schema: "agent");
    }
}
