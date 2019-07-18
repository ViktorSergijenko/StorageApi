using Microsoft.EntityFrameworkCore.Migrations;

namespace StorageAPI.Migrations
{
    public partial class fixingrestrictedlogdelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SimpleLogTableDB_WarehouseDB_WarehouseId",
                table: "SimpleLogTableDB");

            migrationBuilder.AddForeignKey(
                name: "FK_SimpleLogTableDB_WarehouseDB_WarehouseId",
                table: "SimpleLogTableDB",
                column: "WarehouseId",
                principalTable: "WarehouseDB",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SimpleLogTableDB_WarehouseDB_WarehouseId",
                table: "SimpleLogTableDB");

            migrationBuilder.AddForeignKey(
                name: "FK_SimpleLogTableDB_WarehouseDB_WarehouseId",
                table: "SimpleLogTableDB",
                column: "WarehouseId",
                principalTable: "WarehouseDB",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
