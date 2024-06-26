using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NTT_DMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLogsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "N_Log",
                columns: table => new
                {
                    LogID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActionID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ColumnName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginalValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created_by = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Created_date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_N_Log", x => x.LogID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "N_Log");
        }
    }
}
