using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddContactNextAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NextAction",
                table: "contacts",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "NextActionDate",
                table: "contacts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_contacts_NextActionDate",
                table: "contacts",
                column: "NextActionDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_contacts_NextActionDate",
                table: "contacts");

            migrationBuilder.DropColumn(
                name: "NextAction",
                table: "contacts");

            migrationBuilder.DropColumn(
                name: "NextActionDate",
                table: "contacts");
        }
    }
}
