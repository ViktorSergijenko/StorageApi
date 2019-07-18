using Microsoft.EntityFrameworkCore.Migrations;

namespace StorageAPI.Migrations
{
    public partial class Warehouserestrictdelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatalogDB_WarehouseDB_WarehouseId",
                table: "CatalogDB");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogDB_WarehouseDB_WarehouseId",
                table: "CatalogDB",
                column: "WarehouseId",
                principalTable: "WarehouseDB",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatalogDB_WarehouseDB_WarehouseId",
                table: "CatalogDB");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogDB_WarehouseDB_WarehouseId",
                table: "CatalogDB",
                column: "WarehouseId",
                principalTable: "WarehouseDB",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
