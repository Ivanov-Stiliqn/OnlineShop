using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class ChangedLogicForSeenOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WasSeen",
                table: "Orders",
                newName: "NotifySeller");

            migrationBuilder.AddColumn<bool>(
                name: "NotifyBuyer",
                table: "Orders",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotifyBuyer",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "NotifySeller",
                table: "Orders",
                newName: "WasSeen");
        }
    }
}
