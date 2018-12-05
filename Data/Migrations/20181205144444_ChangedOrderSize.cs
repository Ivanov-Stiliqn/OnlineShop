using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class ChangedOrderSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Sizes_SizeId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_SizeId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Size",
                table: "Orders");

            migrationBuilder.AddColumn<Guid>(
                name: "SizeId",
                table: "Orders",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SizeId",
                table: "Orders",
                column: "SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Sizes_SizeId",
                table: "Orders",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
