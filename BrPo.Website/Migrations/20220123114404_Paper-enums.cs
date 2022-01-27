using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BrPo.Website.Migrations
{
    public partial class Paperenums : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Keywords",
                table: "ImageGalleryItems",
                newName: "Tags");

            migrationBuilder.AddColumn<int>(
                name: "PaperSurface",
                table: "Papers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaperTexture",
                table: "Papers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "MinPrice",
                table: "ImageGalleryItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<int>(
                name: "MaxPrice",
                table: "ImageGalleryItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ImageGalleryItems",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "PaperSurface",
                table: "ImageGalleryItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaperTexture",
                table: "ImageGalleryItems",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "746bd60a-5483-43fa-855d-b3bdb085eb0a", "53d78a6a-1f5e-4e06-b8b6-141a769f3dfc" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "Name" },
                values: new object[] { new DateTime(2022, 1, 23, 11, 44, 4, 184, DateTimeKind.Utc).AddTicks(5550), "Ilford Galerie Prestige Smooth Gloss - roll" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateCreated", "Name", "PaperSurface" },
                values: new object[] { new DateTime(2022, 1, 23, 11, 44, 4, 184, DateTimeKind.Utc).AddTicks(8312), "Ilford Galerie Graphic Heavyweight Matt - roll", 1 });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateCreated", "Name", "PaperTexture" },
                values: new object[] { new DateTime(2022, 1, 23, 11, 44, 4, 184, DateTimeKind.Utc).AddTicks(8749), "Ilford Galerie Smooth Pearl - roll", 1 });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DateCreated", "Name", "PaperSurface" },
                values: new object[] { new DateTime(2022, 1, 23, 11, 44, 4, 184, DateTimeKind.Utc).AddTicks(8753), "Olmec Photo Matt Archival - roll", 1 });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DateCreated", "Name", "PaperSurface" },
                values: new object[] { new DateTime(2022, 1, 23, 11, 44, 4, 184, DateTimeKind.Utc).AddTicks(8756), "Matt Self-adhesive Poly-vinyl - roll", 1 });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "DateCreated", "PaperSurface", "PaperTexture" },
                values: new object[] { new DateTime(2022, 1, 23, 11, 44, 4, 184, DateTimeKind.Utc).AddTicks(8758), 2, 3 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaperSurface",
                table: "Papers");

            migrationBuilder.DropColumn(
                name: "PaperTexture",
                table: "Papers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ImageGalleryItems");

            migrationBuilder.DropColumn(
                name: "PaperSurface",
                table: "ImageGalleryItems");

            migrationBuilder.DropColumn(
                name: "PaperTexture",
                table: "ImageGalleryItems");

            migrationBuilder.RenameColumn(
                name: "Tags",
                table: "ImageGalleryItems",
                newName: "Keywords");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinPrice",
                table: "ImageGalleryItems",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxPrice",
                table: "ImageGalleryItems",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "d9fdd57f-f113-4741-9a9a-7d91793f941a", "32bc5930-9f1c-4dc8-a08f-9124e483ad34" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "Name" },
                values: new object[] { new DateTime(2022, 1, 20, 11, 54, 59, 652, DateTimeKind.Utc).AddTicks(1665), "Ilford Galerie Prestige Smooth Gloss - roll paper" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateCreated", "Name" },
                values: new object[] { new DateTime(2022, 1, 20, 11, 54, 59, 652, DateTimeKind.Utc).AddTicks(3187), "Ilford Galerie Graphic Heavyweight Matt - roll paper" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateCreated", "Name" },
                values: new object[] { new DateTime(2022, 1, 20, 11, 54, 59, 652, DateTimeKind.Utc).AddTicks(3519), "Ilford Galerie Smooth Pearl - roll paper" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DateCreated", "Name" },
                values: new object[] { new DateTime(2022, 1, 20, 11, 54, 59, 652, DateTimeKind.Utc).AddTicks(3523), "Olmec Photo Matt Archival - roll paper" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DateCreated", "Name" },
                values: new object[] { new DateTime(2022, 1, 20, 11, 54, 59, 652, DateTimeKind.Utc).AddTicks(3524), "Matt Self-adhesive Poly-vinyl - roll paper" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 6,
                column: "DateCreated",
                value: new DateTime(2022, 1, 20, 11, 54, 59, 652, DateTimeKind.Utc).AddTicks(3526));
        }
    }
}
