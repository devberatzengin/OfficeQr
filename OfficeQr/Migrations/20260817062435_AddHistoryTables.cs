using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeQr.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Cabinets_CabinetId",
                schema: "identity",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Shelves_Cabinets_CabinetId",
                schema: "identity",
                table: "Shelves");

            migrationBuilder.DropIndex(
                name: "IX_Items_CabinetId",
                schema: "identity",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CabinetId",
                schema: "identity",
                table: "Items");

            migrationBuilder.CreateTable(
                name: "ItemShelfHistories",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShelfId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlacedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RemovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemShelfHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemShelfHistories_Items_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "identity",
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemShelfHistories_Shelves_ShelfId",
                        column: x => x.ShelfId,
                        principalSchema: "identity",
                        principalTable: "Shelves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemUserHistories",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReturnedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemUserHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemUserHistories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemUserHistories_Items_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "identity",
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShelfCabinetHistories",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShelfId = table.Column<Guid>(type: "uuid", nullable: false),
                    CabinetId = table.Column<Guid>(type: "uuid", nullable: false),
                    MovedInAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MovedOutAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShelfCabinetHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShelfCabinetHistories_Cabinets_CabinetId",
                        column: x => x.CabinetId,
                        principalSchema: "identity",
                        principalTable: "Cabinets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShelfCabinetHistories_Shelves_ShelfId",
                        column: x => x.ShelfId,
                        principalSchema: "identity",
                        principalTable: "Shelves",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemShelfHistories_ItemId",
                schema: "identity",
                table: "ItemShelfHistories",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemShelfHistories_ShelfId",
                schema: "identity",
                table: "ItemShelfHistories",
                column: "ShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemUserHistories_ItemId",
                schema: "identity",
                table: "ItemUserHistories",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemUserHistories_UserId",
                schema: "identity",
                table: "ItemUserHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShelfCabinetHistories_CabinetId",
                schema: "identity",
                table: "ShelfCabinetHistories",
                column: "CabinetId");

            migrationBuilder.CreateIndex(
                name: "IX_ShelfCabinetHistories_ShelfId",
                schema: "identity",
                table: "ShelfCabinetHistories",
                column: "ShelfId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shelves_Cabinets_CabinetId",
                schema: "identity",
                table: "Shelves");

            migrationBuilder.DropTable(
                name: "ItemShelfHistories",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "ItemUserHistories",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "ShelfCabinetHistories",
                schema: "identity");

            migrationBuilder.AddColumn<Guid>(
                name: "CabinetId",
                schema: "identity",
                table: "Items",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_CabinetId",
                schema: "identity",
                table: "Items",
                column: "CabinetId");

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
                name: "FK_Shelves_Cabinets_CabinetId",
                schema: "identity",
                table: "Shelves",
                column: "CabinetId",
                principalSchema: "identity",
                principalTable: "Cabinets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
