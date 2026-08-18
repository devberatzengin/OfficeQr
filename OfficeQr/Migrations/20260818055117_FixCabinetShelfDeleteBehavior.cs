using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeQr.Migrations
{
    /// <inheritdoc />
    public partial class FixCabinetShelfDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shelves_Cabinets_CabinetId",
                schema: "identity",
                table: "Shelves");

            migrationBuilder.AddForeignKey(
                name: "FK_Shelves_Cabinets_CabinetId",
                schema: "identity",
                table: "Shelves",
                column: "CabinetId",
                principalSchema: "identity",
                principalTable: "Cabinets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shelves_Cabinets_CabinetId",
                schema: "identity",
                table: "Shelves");

            migrationBuilder.AddForeignKey(
                name: "FK_Shelves_Cabinets_CabinetId",
                schema: "identity",
                table: "Shelves",
                column: "CabinetId",
                principalSchema: "identity",
                principalTable: "Cabinets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
