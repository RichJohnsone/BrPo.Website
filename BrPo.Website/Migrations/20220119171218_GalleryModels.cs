using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BrPo.Website.Migrations
{
    public partial class GalleryModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImageGalleries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ImageFileId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageGalleries", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ImageGalleryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ImageFileId = table.Column<int>(type: "int", nullable: false),
                    MinHeight = table.Column<int>(type: "int", nullable: false),
                    MinWidth = table.Column<int>(type: "int", nullable: false),
                    MaxHeight = table.Column<int>(type: "int", nullable: false),
                    MaxWidth = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Keywords = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MinPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    MaxPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageGalleryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageGalleryItems_ImageFiles_ImageFileId",
                        column: x => x.ImageFileId,
                        principalTable: "ImageFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                columns: new[] { "DateCreated", "Name" },
                values: new object[] { new DateTime(2022, 1, 19, 17, 12, 17, 995, DateTimeKind.Utc).AddTicks(5806), "Ilford Galerie Prestige Smooth Gloss - roll paper" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateCreated", "Name" },
                values: new object[] { new DateTime(2022, 1, 19, 17, 12, 17, 995, DateTimeKind.Utc).AddTicks(7972), "Ilford Galerie Graphic Heavyweight Matt - roll paper" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateCreated", "Name" },
                values: new object[] { new DateTime(2022, 1, 19, 17, 12, 17, 995, DateTimeKind.Utc).AddTicks(8427), "Ilford Galerie Smooth Pearl - roll paper" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DateCreated", "Name" },
                values: new object[] { new DateTime(2022, 1, 19, 17, 12, 17, 995, DateTimeKind.Utc).AddTicks(8433), "Olmec Photo Matt Archival - roll paper" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DateCreated", "Name" },
                values: new object[] { new DateTime(2022, 1, 19, 17, 12, 17, 995, DateTimeKind.Utc).AddTicks(8436), "Matt Self-adhesive Poly-vinyl - roll paper" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 6,
                column: "DateCreated",
                value: new DateTime(2022, 1, 19, 17, 12, 17, 995, DateTimeKind.Utc).AddTicks(8438));

            migrationBuilder.CreateIndex(
                name: "IX_ImageGalleryItems_ImageFileId",
                table: "ImageGalleryItems",
                column: "ImageFileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageGalleries");

            migrationBuilder.DropTable(
                name: "ImageGalleryItems");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "e30a27f8-a0fc-4216-891b-a5d4c6bb82d6", "1dc46160-0547-4d3e-b83b-9db1ce4d4b05" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DateCreated", "Name" },
                values: new object[] { new DateTime(2022, 1, 19, 10, 29, 11, 958, DateTimeKind.Utc).AddTicks(7251), "Ilford Galerie Prestige Smooth Gloss - 17 roll" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DateCreated", "Name" },
                values: new object[] { new DateTime(2022, 1, 19, 10, 29, 11, 958, DateTimeKind.Utc).AddTicks(9393), "Ilford Galerie Graphic Heavyweight Matt - 17 roll" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DateCreated", "Name" },
                values: new object[] { new DateTime(2022, 1, 19, 10, 29, 11, 958, DateTimeKind.Utc).AddTicks(9840), "Ilford Galerie Smooth Pearl - 17 roll" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "DateCreated", "Name" },
                values: new object[] { new DateTime(2022, 1, 19, 10, 29, 11, 958, DateTimeKind.Utc).AddTicks(9845), "Olmec Photo Matt Archival - 17 roll" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "DateCreated", "Name" },
                values: new object[] { new DateTime(2022, 1, 19, 10, 29, 11, 958, DateTimeKind.Utc).AddTicks(9847), "Matt Self-adhesive Poly-vinyl - 17 roll" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 6,
                column: "DateCreated",
                value: new DateTime(2022, 1, 19, 10, 29, 11, 958, DateTimeKind.Utc).AddTicks(9850));
        }
    }
}
