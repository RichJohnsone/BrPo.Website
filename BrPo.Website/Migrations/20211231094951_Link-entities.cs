using Microsoft.EntityFrameworkCore.Migrations;

namespace BrPo.Website.Migrations
{
    public partial class Linkentities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_basketItems_PrintOrderId",
                table: "basketItems",
                column: "PrintOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_basketItems_PrintOrders_PrintOrderId",
                table: "basketItems",
                column: "PrintOrderId",
                principalTable: "PrintOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_basketItems_PrintOrders_PrintOrderId",
                table: "basketItems");

            migrationBuilder.DropIndex(
                name: "IX_basketItems_PrintOrderId",
                table: "basketItems");
        }
    }
}
