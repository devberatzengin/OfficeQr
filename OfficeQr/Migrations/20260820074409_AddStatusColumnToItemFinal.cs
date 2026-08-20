using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeQr.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusColumnToItemFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "identity",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                schema: "identity",
                table: "Items");
        }
    }
}
