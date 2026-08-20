using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeQr.Migrations
{
    /// <inheritdoc />
    public partial class AddItemVersionConcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "identity",
                table: "Items",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "identity",
                table: "Items");
        }
    }
}
