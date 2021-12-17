using Microsoft.EntityFrameworkCore.Migrations;

namespace BrPo.Website.Migrations
{
    public partial class addorientationtoimageModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Orientation",
                table: "ImageFiles",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Orientation",
                table: "ImageFiles");
        }
    }
}
