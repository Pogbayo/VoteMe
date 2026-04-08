using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoteMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedEmailPropertyFromorganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Organizations_Email",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Organizations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Organizations",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Email",
                table: "Organizations",
                column: "Email",
                unique: true);
        }
    }
}
