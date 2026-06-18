using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Notification.DrivenInfrastructure.Migrations;

/// <inheritdoc />
public partial class AddProjectPermissionsReadModel : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "level",
            schema: "notification",
            table: "project_permission_read_models",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "permission_name",
            schema: "notification",
            table: "project_permission_read_models",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<Guid>(
            name: "project_id",
            schema: "notification",
            table: "project_permission_read_models",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);

        migrationBuilder.AddColumn<Guid>(
            name: "user_id",
            schema: "notification",
            table: "project_permission_read_models",
            type: "uuid",
            nullable: false,
            defaultValue: Guid.Empty);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "level",
            schema: "notification",
            table: "project_permission_read_models");

        migrationBuilder.DropColumn(
            name: "permission_name",
            schema: "notification",
            table: "project_permission_read_models");

        migrationBuilder.DropColumn(
            name: "project_id",
            schema: "notification",
            table: "project_permission_read_models");

        migrationBuilder.DropColumn(
            name: "user_id",
            schema: "notification",
            table: "project_permission_read_models");
    }
}
