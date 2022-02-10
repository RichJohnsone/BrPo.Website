using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrPo.Website.Migrations
{
    public partial class AddIsActiveToGallery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ImageGalleries",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ImageGalleries");
        }
    }
}
