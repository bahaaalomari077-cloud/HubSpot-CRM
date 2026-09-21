using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Crm.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityContactCreatedAtIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_activities_ContactId",
                table: "activities");

            migrationBuilder.CreateIndex(
                name: "IX_activities_ContactId_CreatedAt",
                table: "activities",
                columns: new[] { "ContactId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_activities_ContactId_CreatedAt",
                table: "activities");

            migrationBuilder.CreateIndex(
                name: "IX_activities_ContactId",
                table: "activities",
                column: "ContactId");
        }
    }
}
