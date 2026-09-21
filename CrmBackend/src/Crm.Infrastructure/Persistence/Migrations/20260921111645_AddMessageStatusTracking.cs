using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageStatusTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttemptCount",
                table: "messages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "FailureReason",
                table: "messages",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastAttemptAt",
                table: "messages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderMessageId",
                table: "messages",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Recipient",
                table: "messages",
                type: "character varying(320)",
                maxLength: 320,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "messages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_messages_ProviderMessageId",
                table: "messages",
                column: "ProviderMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_messages_Status",
                table: "messages",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_messages_ProviderMessageId",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "IX_messages_Status",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "AttemptCount",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "FailureReason",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "LastAttemptAt",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "ProviderMessageId",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "Recipient",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "messages");
        }
    }
}
