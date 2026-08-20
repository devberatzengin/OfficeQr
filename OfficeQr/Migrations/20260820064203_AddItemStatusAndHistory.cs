using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeQr.Migrations
{
    /// <inheritdoc />
    public partial class AddItemStatusAndHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemStatusHistories",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemStatusHistories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemStatusHistories_Items_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "identity",
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemStatusHistories_ItemId_ChangedAt",
                schema: "identity",
                table: "ItemStatusHistories",
                columns: new[] { "ItemId", "ChangedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ItemStatusHistories_UserId_ChangedAt",
                schema: "identity",
                table: "ItemStatusHistories",
                columns: new[] { "UserId", "ChangedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemStatusHistories",
                schema: "identity");
        }
    }
}
