using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OfficeQr.Migrations
{
    /// <inheritdoc />
    public partial class AddActorAndCloseReasonToHistoryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssignedByUserId",
                schema: "identity",
                table: "ItemUserHistories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReturnedByUserId",
                schema: "identity",
                table: "ItemUserHistories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReturnedReason",
                schema: "identity",
                table: "ItemUserHistories",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlacedByUserId",
                schema: "identity",
                table: "ItemShelfHistories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RemovedByUserId",
                schema: "identity",
                table: "ItemShelfHistories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemovedReason",
                schema: "identity",
                table: "ItemShelfHistories",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemUserHistories_AssignedByUserId",
                schema: "identity",
                table: "ItemUserHistories",
                column: "AssignedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemUserHistories_ReturnedByUserId",
                schema: "identity",
                table: "ItemUserHistories",
                column: "ReturnedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemShelfHistories_PlacedByUserId",
                schema: "identity",
                table: "ItemShelfHistories",
                column: "PlacedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemShelfHistories_RemovedByUserId",
                schema: "identity",
                table: "ItemShelfHistories",
                column: "RemovedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemShelfHistories_AspNetUsers_PlacedByUserId",
                schema: "identity",
                table: "ItemShelfHistories",
                column: "PlacedByUserId",
                principalSchema: "identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemShelfHistories_AspNetUsers_RemovedByUserId",
                schema: "identity",
                table: "ItemShelfHistories",
                column: "RemovedByUserId",
                principalSchema: "identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemUserHistories_AspNetUsers_AssignedByUserId",
                schema: "identity",
                table: "ItemUserHistories",
                column: "AssignedByUserId",
                principalSchema: "identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemUserHistories_AspNetUsers_ReturnedByUserId",
                schema: "identity",
                table: "ItemUserHistories",
                column: "ReturnedByUserId",
                principalSchema: "identity",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemShelfHistories_AspNetUsers_PlacedByUserId",
                schema: "identity",
                table: "ItemShelfHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemShelfHistories_AspNetUsers_RemovedByUserId",
                schema: "identity",
                table: "ItemShelfHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemUserHistories_AspNetUsers_AssignedByUserId",
                schema: "identity",
                table: "ItemUserHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemUserHistories_AspNetUsers_ReturnedByUserId",
                schema: "identity",
                table: "ItemUserHistories");

            migrationBuilder.DropIndex(
                name: "IX_ItemUserHistories_AssignedByUserId",
                schema: "identity",
                table: "ItemUserHistories");

            migrationBuilder.DropIndex(
                name: "IX_ItemUserHistories_ReturnedByUserId",
                schema: "identity",
                table: "ItemUserHistories");

            migrationBuilder.DropIndex(
                name: "IX_ItemShelfHistories_PlacedByUserId",
                schema: "identity",
                table: "ItemShelfHistories");

            migrationBuilder.DropIndex(
                name: "IX_ItemShelfHistories_RemovedByUserId",
                schema: "identity",
                table: "ItemShelfHistories");

            migrationBuilder.DropColumn(
                name: "AssignedByUserId",
                schema: "identity",
                table: "ItemUserHistories");

            migrationBuilder.DropColumn(
                name: "ReturnedByUserId",
                schema: "identity",
                table: "ItemUserHistories");

            migrationBuilder.DropColumn(
                name: "ReturnedReason",
                schema: "identity",
                table: "ItemUserHistories");

            migrationBuilder.DropColumn(
                name: "PlacedByUserId",
                schema: "identity",
                table: "ItemShelfHistories");

            migrationBuilder.DropColumn(
                name: "RemovedByUserId",
                schema: "identity",
                table: "ItemShelfHistories");

            migrationBuilder.DropColumn(
                name: "RemovedReason",
                schema: "identity",
                table: "ItemShelfHistories");
        }
    }
}
