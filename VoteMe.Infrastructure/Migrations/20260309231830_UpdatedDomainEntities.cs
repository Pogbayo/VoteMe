using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoteMe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDomainEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Elections_ElectionId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_VoterId_ElectionId",
                table: "Votes");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Elections",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Candidates",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "Tokenversion",
                table: "AspNetUsers",
                newName: "TokenVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "ElectionCategoryId",
                table: "Votes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Bio",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ElectionCategoryId",
                table: "Candidates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ElectionCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ElectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectionCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElectionCategories_Elections_ElectionId",
                        column: x => x.ElectionId,
                        principalTable: "Elections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Votes_ElectionCategoryId",
                table: "Votes",
                column: "ElectionCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_VoterId_ElectionCategoryId",
                table: "Votes",
                columns: new[] { "VoterId", "ElectionCategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_ElectionCategoryId",
                table: "Candidates",
                column: "ElectionCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ElectionCategories_ElectionId",
                table: "ElectionCategories",
                column: "ElectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidates_ElectionCategories_ElectionCategoryId",
                table: "Candidates",
                column: "ElectionCategoryId",
                principalTable: "ElectionCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_ElectionCategories_ElectionCategoryId",
                table: "Votes",
                column: "ElectionCategoryId",
                principalTable: "ElectionCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Elections_ElectionId",
                table: "Votes",
                column: "ElectionId",
                principalTable: "Elections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidates_ElectionCategories_ElectionCategoryId",
                table: "Candidates");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_ElectionCategories_ElectionCategoryId",
                table: "Votes");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Elections_ElectionId",
                table: "Votes");

            migrationBuilder.DropTable(
                name: "ElectionCategories");

            migrationBuilder.DropIndex(
                name: "IX_Votes_ElectionCategoryId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_VoterId_ElectionCategoryId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Candidates_ElectionCategoryId",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "ElectionCategoryId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "ElectionCategoryId",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Elections",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Candidates",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "TokenVersion",
                table: "AspNetUsers",
                newName: "Tokenversion");

            migrationBuilder.AlterColumn<string>(
                name: "Bio",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_VoterId_ElectionId",
                table: "Votes",
                columns: new[] { "VoterId", "ElectionId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Elections_ElectionId",
                table: "Votes",
                column: "ElectionId",
                principalTable: "Elections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
