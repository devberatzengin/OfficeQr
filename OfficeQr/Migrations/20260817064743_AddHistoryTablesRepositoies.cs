using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeQr.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoryTablesRepositoies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemShelfHistories_Items_ItemId",
                schema: "identity",
                table: "ItemShelfHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemShelfHistories_Shelves_ShelfId",
                schema: "identity",
                table: "ItemShelfHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemUserHistories_AspNetUsers_UserId",
                schema: "identity",
                table: "ItemUserHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemUserHistories_Items_ItemId",
                schema: "identity",
                table: "ItemUserHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ShelfCabinetHistories_Cabinets_CabinetId",
                schema: "identity",
                table: "ShelfCabinetHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ShelfCabinetHistories_Shelves_ShelfId",
                schema: "identity",
                table: "ShelfCabinetHistories");

            migrationBuilder.DropIndex(
                name: "IX_ShelfCabinetHistories_ShelfId",
                schema: "identity",
                table: "ShelfCabinetHistories");

            migrationBuilder.DropIndex(
                name: "IX_ItemUserHistories_ItemId",
                schema: "identity",
                table: "ItemUserHistories");

            migrationBuilder.DropIndex(
                name: "IX_ItemShelfHistories_ItemId",
                schema: "identity",
                table: "ItemShelfHistories");

            migrationBuilder.CreateIndex(
                name: "IX_ShelfCabinetHistories_ShelfId_MovedOutAt",
                schema: "identity",
                table: "ShelfCabinetHistories",
                columns: new[] { "ShelfId", "MovedOutAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ItemUserHistories_ItemId_ReturnedAt",
                schema: "identity",
                table: "ItemUserHistories",
                columns: new[] { "ItemId", "ReturnedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ItemShelfHistories_ItemId_RemovedAt",
                schema: "identity",
                table: "ItemShelfHistories",
                columns: new[] { "ItemId", "RemovedAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_ItemShelfHistories_Items_ItemId",
                schema: "identity",
                table: "ItemShelfHistories",
                column: "ItemId",
                principalSchema: "identity",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemShelfHistories_Shelves_ShelfId",
                schema: "identity",
                table: "ItemShelfHistories",
                column: "ShelfId",
                principalSchema: "identity",
                principalTable: "Shelves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemUserHistories_AspNetUsers_UserId",
                schema: "identity",
                table: "ItemUserHistories",
                column: "UserId",
                principalSchema: "identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemUserHistories_Items_ItemId",
                schema: "identity",
                table: "ItemUserHistories",
                column: "ItemId",
                principalSchema: "identity",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShelfCabinetHistories_Cabinets_CabinetId",
                schema: "identity",
                table: "ShelfCabinetHistories",
                column: "CabinetId",
                principalSchema: "identity",
                principalTable: "Cabinets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShelfCabinetHistories_Shelves_ShelfId",
                schema: "identity",
                table: "ShelfCabinetHistories",
                column: "ShelfId",
                principalSchema: "identity",
                principalTable: "Shelves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemShelfHistories_Items_ItemId",
                schema: "identity",
                table: "ItemShelfHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemShelfHistories_Shelves_ShelfId",
                schema: "identity",
                table: "ItemShelfHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemUserHistories_AspNetUsers_UserId",
                schema: "identity",
                table: "ItemUserHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemUserHistories_Items_ItemId",
                schema: "identity",
                table: "ItemUserHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ShelfCabinetHistories_Cabinets_CabinetId",
                schema: "identity",
                table: "ShelfCabinetHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ShelfCabinetHistories_Shelves_ShelfId",
                schema: "identity",
                table: "ShelfCabinetHistories");

            migrationBuilder.DropIndex(
                name: "IX_ShelfCabinetHistories_ShelfId_MovedOutAt",
                schema: "identity",
                table: "ShelfCabinetHistories");

            migrationBuilder.DropIndex(
                name: "IX_ItemUserHistories_ItemId_ReturnedAt",
                schema: "identity",
                table: "ItemUserHistories");

            migrationBuilder.DropIndex(
                name: "IX_ItemShelfHistories_ItemId_RemovedAt",
                schema: "identity",
                table: "ItemShelfHistories");

            migrationBuilder.CreateIndex(
                name: "IX_ShelfCabinetHistories_ShelfId",
                schema: "identity",
                table: "ShelfCabinetHistories",
                column: "ShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemUserHistories_ItemId",
                schema: "identity",
                table: "ItemUserHistories",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemShelfHistories_ItemId",
                schema: "identity",
                table: "ItemShelfHistories",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemShelfHistories_Items_ItemId",
                schema: "identity",
                table: "ItemShelfHistories",
                column: "ItemId",
                principalSchema: "identity",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemShelfHistories_Shelves_ShelfId",
                schema: "identity",
                table: "ItemShelfHistories",
                column: "ShelfId",
                principalSchema: "identity",
                principalTable: "Shelves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemUserHistories_AspNetUsers_UserId",
                schema: "identity",
                table: "ItemUserHistories",
                column: "UserId",
                principalSchema: "identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemUserHistories_Items_ItemId",
                schema: "identity",
                table: "ItemUserHistories",
                column: "ItemId",
                principalSchema: "identity",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShelfCabinetHistories_Cabinets_CabinetId",
                schema: "identity",
                table: "ShelfCabinetHistories",
                column: "CabinetId",
                principalSchema: "identity",
                principalTable: "Cabinets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShelfCabinetHistories_Shelves_ShelfId",
                schema: "identity",
                table: "ShelfCabinetHistories",
                column: "ShelfId",
                principalSchema: "identity",
                principalTable: "Shelves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
