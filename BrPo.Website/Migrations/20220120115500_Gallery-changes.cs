using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BrPo.Website.Migrations
{
    public partial class Gallerychanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFileId",
                table: "ImageGalleries");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ImageGalleries",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ImageGalleryContent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ImageGalleryId = table.Column<int>(type: "int", nullable: false),
                    ImageGalleryItemId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime(0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageGalleryContent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageGalleryContent_ImageGalleries_ImageGalleryId",
                        column: x => x.ImageGalleryId,
                        principalTable: "ImageGalleries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ImageGalleryContent_ImageGalleryItems_ImageGalleryItemId",
                        column: x => x.ImageGalleryItemId,
                        principalTable: "ImageGalleryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                column: "DateCreated",
                value: new DateTime(2022, 1, 20, 11, 54, 59, 652, DateTimeKind.Utc).AddTicks(1665));

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2022, 1, 20, 11, 54, 59, 652, DateTimeKind.Utc).AddTicks(3187));

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2022, 1, 20, 11, 54, 59, 652, DateTimeKind.Utc).AddTicks(3519));

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2022, 1, 20, 11, 54, 59, 652, DateTimeKind.Utc).AddTicks(3523));

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 5,
                column: "DateCreated",
                value: new DateTime(2022, 1, 20, 11, 54, 59, 652, DateTimeKind.Utc).AddTicks(3524));

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 6,
                column: "DateCreated",
                value: new DateTime(2022, 1, 20, 11, 54, 59, 652, DateTimeKind.Utc).AddTicks(3526));

            migrationBuilder.CreateIndex(
                name: "IX_ImageGalleryContent_ImageGalleryId",
                table: "ImageGalleryContent",
                column: "ImageGalleryId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageGalleryContent_ImageGalleryItemId",
                table: "ImageGalleryContent",
                column: "ImageGalleryItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageGalleryContent");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ImageGalleries");

            migrationBuilder.AddColumn<int>(
                name: "ImageFileId",
                table: "ImageGalleries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "003243d2-e42b-43c0-9600-1d0b98de6429", "62d6d667-35c0-461f-92b3-cc9ae682625c" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2022, 1, 19, 17, 12, 17, 995, DateTimeKind.Utc).AddTicks(5806));

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2022, 1, 19, 17, 12, 17, 995, DateTimeKind.Utc).AddTicks(7972));

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2022, 1, 19, 17, 12, 17, 995, DateTimeKind.Utc).AddTicks(8427));

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2022, 1, 19, 17, 12, 17, 995, DateTimeKind.Utc).AddTicks(8433));

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 5,
                column: "DateCreated",
                value: new DateTime(2022, 1, 19, 17, 12, 17, 995, DateTimeKind.Utc).AddTicks(8436));

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 6,
                column: "DateCreated",
                value: new DateTime(2022, 1, 19, 17, 12, 17, 995, DateTimeKind.Utc).AddTicks(8438));
        }
    }
}