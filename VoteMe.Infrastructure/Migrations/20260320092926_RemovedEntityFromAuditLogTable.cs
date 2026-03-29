using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoteMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedEntityFromAuditLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Entity",
                table: "AuditLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Entity",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
