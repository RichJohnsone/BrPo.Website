using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BrPo.Website.Migrations
{
    public partial class ImageTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "ImageGalleryItems");

            migrationBuilder.CreateTable(
                name: "ImageTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ImageGalleryItemId = table.Column<int>(type: "int", nullable: false),
                    Tag = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateCreated = table.Column<DateTime>(type: "datetime(0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageTags_ImageGalleryItems_ImageGalleryItemId",
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
                values: new object[] { "21774353-8358-45da-8d2f-6b827072b713", "680e67fd-8cd7-416b-a727-93fb8cf7c5a5" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2022, 1, 24, 13, 42, 49, 728, DateTimeKind.Utc).AddTicks(4989));

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2022, 1, 24, 13, 42, 49, 728, DateTimeKind.Utc).AddTicks(6961));

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2022, 1, 24, 13, 42, 49, 728, DateTimeKind.Utc).AddTicks(7441));

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2022, 1, 24, 13, 42, 49, 728, DateTimeKind.Utc).AddTicks(7446));

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 5,
                column: "DateCreated",
                value: new DateTime(2022, 1, 24, 13, 42, 49, 728, DateTimeKind.Utc).AddTicks(7448));

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 6,
                column: "DateCreated",
                value: new DateTime(2022, 1, 24, 13, 42, 49, 728, DateTimeKind.Utc).AddTicks(7450));

            migrationBuilder.CreateIndex(
                name: "IX_ImageTags_ImageGalleryItemId",
                table: "ImageTags",
                column: "ImageGalleryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageTags_Tag",
                table: "ImageTags",
                column: "Tag");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageTags");

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "ImageGalleryItems",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

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
                column: "DateCreated",
                value: new DateTime(2022, 1, 23, 11, 44, 4, 184, DateTimeKind.Utc).AddTicks(5550));

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateCreated",
                value: new DateTime(2022, 1, 23, 11, 44, 4, 184, DateTimeKind.Utc).AddTicks(8312));

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 3,
                column: "DateCreated",
                value: new DateTime(2022, 1, 23, 11, 44, 4, 184, DateTimeKind.Utc).AddTicks(8749));

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 4,
                column: "DateCreated",
                value: new DateTime(2022, 1, 23, 11, 44, 4, 184, DateTimeKind.Utc).AddTicks(8753));

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 5,
                column: "DateCreated",
                value: new DateTime(2022, 1, 23, 11, 44, 4, 184, DateTimeKind.Utc).AddTicks(8756));

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 6,
                column: "DateCreated",
                value: new DateTime(2022, 1, 23, 11, 44, 4, 184, DateTimeKind.Utc).AddTicks(8758));
        }
    }
}