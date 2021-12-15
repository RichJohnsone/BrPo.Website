using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BrPo.Website.Migrations
{
    public partial class ImageFileadditions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColourSpace",
                table: "ImageFiles",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Creator",
                table: "ImageFiles",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Credit",
                table: "ImageFiles",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ImageFiles",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ImageCreatedDate",
                table: "ImageFiles",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Keywords",
                table: "ImageFiles",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColourSpace",
                table: "ImageFiles");

            migrationBuilder.DropColumn(
                name: "Creator",
                table: "ImageFiles");

            migrationBuilder.DropColumn(
                name: "Credit",
                table: "ImageFiles");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ImageFiles");

            migrationBuilder.DropColumn(
                name: "ImageCreatedDate",
                table: "ImageFiles");

            migrationBuilder.DropColumn(
                name: "Keywords",
                table: "ImageFiles");
        }
    }
}
