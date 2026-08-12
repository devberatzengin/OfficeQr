using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeQr.Migrations
{
    /// <inheritdoc />
    public partial class EntitiesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_AspNetUsers_UserId",
                schema: "identity",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Cabinets_CabinetId",
                schema: "identity",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Shelves_ShelfId",
                schema: "identity",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Shelves_Cabinets_CabinetId",
                schema: "identity",
                table: "Shelves");

            migrationBuilder.CreateIndex(
                name: "IX_Shelves_QrCode",
                schema: "identity",
                table: "Shelves",
                column: "QrCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_QrCode",
                schema: "identity",
                table: "Items",
                column: "QrCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cabinets_QrCode",
                schema: "identity",
                table: "Cabinets",
                column: "QrCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_AspNetUsers_UserId",
                schema: "identity",
                table: "Items",
                column: "UserId",
                principalSchema: "identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Cabinets_CabinetId",
                schema: "identity",
                table: "Items",
                column: "CabinetId",
                principalSchema: "identity",
                principalTable: "Cabinets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Shelves_ShelfId",
                schema: "identity",
                table: "Items",
                column: "ShelfId",
                principalSchema: "identity",
                principalTable: "Shelves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_Items_AspNetUsers_UserId",
                schema: "identity",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Cabinets_CabinetId",
                schema: "identity",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Shelves_ShelfId",
                schema: "identity",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Shelves_Cabinets_CabinetId",
                schema: "identity",
                table: "Shelves");

            migrationBuilder.DropIndex(
                name: "IX_Shelves_QrCode",
                schema: "identity",
                table: "Shelves");

            migrationBuilder.DropIndex(
                name: "IX_Items_QrCode",
                schema: "identity",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Cabinets_QrCode",
                schema: "identity",
                table: "Cabinets");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_AspNetUsers_UserId",
                schema: "identity",
                table: "Items",
                column: "UserId",
                principalSchema: "identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Cabinets_CabinetId",
                schema: "identity",
                table: "Items",
                column: "CabinetId",
                principalSchema: "identity",
                principalTable: "Cabinets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Shelves_ShelfId",
                schema: "identity",
                table: "Items",
                column: "ShelfId",
                principalSchema: "identity",
                principalTable: "Shelves",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Shelves_Cabinets_CabinetId",
                schema: "identity",
                table: "Shelves",
                column: "CabinetId",
                principalSchema: "identity",
                principalTable: "Cabinets",
                principalColumn: "Id");
        }
    }
}
