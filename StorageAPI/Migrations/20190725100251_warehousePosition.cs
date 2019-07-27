using Microsoft.EntityFrameworkCore.Migrations;

namespace StorageAPI.Migrations
{
    public partial class warehousePosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WarehousePositionInTable",
                table: "UserWarehouseDB",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WarehousePositionInTable",
                table: "UserWarehouseDB");
        }
    }
}
