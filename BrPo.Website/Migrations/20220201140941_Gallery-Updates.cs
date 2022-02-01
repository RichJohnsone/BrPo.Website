using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrPo.Website.Migrations
{
    public partial class GalleryUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "ImageGalleryItems",
                newName: "DateUpdated");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "UserDetails",
                type: "datetime(0)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "GalleryRootName",
                table: "UserDetails",
                type: "varchar(30)",
                maxLength: 30,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ImageTags",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "ContentCount",
                table: "ImageGalleries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CoverImageId",
                table: "ImageGalleries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdated",
                table: "ImageGalleries",
                type: "datetime(0)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "GalleryRootName",
                table: "ImageGalleries",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "GalleryRootName",
                table: "UserDetails");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ImageTags");

            migrationBuilder.DropColumn(
                name: "ContentCount",
                table: "ImageGalleries");

            migrationBuilder.DropColumn(
                name: "CoverImageId",
                table: "ImageGalleries");

            migrationBuilder.DropColumn(
                name: "DateUpdated",
                table: "ImageGalleries");

            migrationBuilder.DropColumn(
                name: "GalleryRootName",
                table: "ImageGalleries");

            migrationBuilder.RenameColumn(
                name: "DateUpdated",
                table: "ImageGalleryItems",
                newName: "UpdatedDate");
        }
    }
}
