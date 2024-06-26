using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NTT_DMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDocumentUsersUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_CatUsersUserId",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "CatUsersUserId",
                table: "Documents",
                newName: "UsersUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_CatUsersUserId",
                table: "Documents",
                newName: "IX_Documents_UsersUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users_UsersUserId",
                table: "Documents",
                column: "UsersUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Users_UsersUserId",
                table: "Documents");

            migrationBuilder.RenameColumn(
                name: "UsersUserId",
                table: "Documents",
                newName: "CatUsersUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Documents_UsersUserId",
                table: "Documents",
                newName: "IX_Documents_CatUsersUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Users_CatUsersUserId",
                table: "Documents",
                column: "CatUsersUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
