using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Chats.DrivenInfrastructure.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    private static readonly string[] ChatParticipantUniqueColumns = ["chat_id", "participant_id"];

    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "chat");

        migrationBuilder.CreateTable(
            name: "chats",
            schema: "chat",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_chats", x => x.Id));

        migrationBuilder.CreateTable(
            name: "InboxMessages",
            schema: "chat",
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
            schema: "chat",
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
            name: "user_read_models",
            schema: "chat",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_user_read_models", x => x.Id));

        migrationBuilder.CreateTable(
            name: "chat_participants",
            schema: "chat",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                participant_id = table.Column<Guid>(type: "uuid", nullable: false),
                participant_type = table.Column<int>(type: "integer", nullable: false),
                last_fetched = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                chat_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_chat_participants", x => x.Id);
                table.ForeignKey(
                    name: "FK_chat_participants_chats_chat_id",
                    column: x => x.chat_id,
                    principalSchema: "chat",
                    principalTable: "chats",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "chat_messages",
            schema: "chat",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                content = table.Column<string>(type: "text", nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false),
                chat_id = table.Column<Guid>(type: "uuid", nullable: false),
                sender_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_chat_messages", x => x.Id);
                table.ForeignKey(
                    name: "FK_chat_messages_chat_participants_sender_id",
                    column: x => x.sender_id,
                    principalSchema: "chat",
                    principalTable: "chat_participants",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_chat_messages_chats_chat_id",
                    column: x => x.chat_id,
                    principalSchema: "chat",
                    principalTable: "chats",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_chat_messages_chat_id",
            schema: "chat",
            table: "chat_messages",
            column: "chat_id");

        migrationBuilder.CreateIndex(
            name: "IX_chat_messages_sender_id",
            schema: "chat",
            table: "chat_messages",
            column: "sender_id");

        migrationBuilder.CreateIndex(
            name: "IX_chat_participants_chat_id_participant_id",
            schema: "chat",
            table: "chat_participants",
            columns: ChatParticipantUniqueColumns,
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "chat_messages",
            schema: "chat");

        migrationBuilder.DropTable(
            name: "InboxMessages",
            schema: "chat");

        migrationBuilder.DropTable(
            name: "OutboxMessages",
            schema: "chat");

        migrationBuilder.DropTable(
            name: "user_read_models",
            schema: "chat");

        migrationBuilder.DropTable(
            name: "chat_participants",
            schema: "chat");

        migrationBuilder.DropTable(
            name: "chats",
            schema: "chat");
    }
}
