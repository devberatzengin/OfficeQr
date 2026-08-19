using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeQr.Migrations
{
    /// <inheritdoc />
    public partial class AddReasonToHistoryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Reason",
                schema: "identity",
                table: "ItemUserHistories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Reason",
                schema: "identity",
                table: "ItemShelfHistories",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                schema: "identity",
                table: "ItemUserHistories");

            migrationBuilder.DropColumn(
                name: "Reason",
                schema: "identity",
                table: "ItemShelfHistories");
        }
    }
}
