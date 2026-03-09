using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoteMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCandidateDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_ElectionCategories_ElectionCategoryId",
                table: "Candidates");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_ElectionCategories_ElectionCategoryId",
                table: "Candidates",
                column: "ElectionCategoryId",
                principalTable: "ElectionCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_ElectionCategories_ElectionCategoryId",
                table: "Candidates");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_ElectionCategories_ElectionCategoryId",
                table: "Candidates",
                column: "ElectionCategoryId",
                principalTable: "ElectionCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
