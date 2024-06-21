using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NTT_DMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class logTableNameChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_N_Log",
                table: "N_Log");

            migrationBuilder.RenameTable(
                name: "N_Log",
                newName: "Logs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Logs",
                table: "Logs",
                column: "LogID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Logs",
                table: "Logs");

            migrationBuilder.RenameTable(
                name: "Logs",
                newName: "N_Log");

            migrationBuilder.AddPrimaryKey(
                name: "PK_N_Log",
                table: "N_Log",
                column: "LogID");
        }
    }
}
