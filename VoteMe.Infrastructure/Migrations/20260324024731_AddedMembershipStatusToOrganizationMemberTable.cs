using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoteMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedMembershipStatusToOrganizationMemberTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "OrganizationMembers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "OrganizationMembers");
        }
    }
}
