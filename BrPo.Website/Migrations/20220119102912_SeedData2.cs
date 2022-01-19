using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BrPo.Website.Migrations
{
    public partial class SeedData2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                columns: new[] { "CostPerMeter", "CostPerSheet", "CutSheetHeight", "CutSheetWidth", "DateCreated", "Description", "GSMWeight", "IsActive", "Name", "ProductCode", "RollPaper", "RollWidth" },
                values: new object[] { 32m, 0m, 0, 0, new DateTime(2022, 1, 19, 10, 29, 11, 958, DateTimeKind.Utc).AddTicks(7251), "natural white smooth gloss surface", 310, true, "Ilford Galerie Prestige Smooth Gloss - 17 roll", "GPSGP12", true, 432 });

            migrationBuilder.InsertData(
                table: "Papers",
                columns: new[] { "Id", "ColourProfilePath", "CostPerMeter", "CostPerSheet", "CutSheetHeight", "CutSheetWidth", "DateCreated", "Description", "GSMWeight", "IsActive", "Name", "ProductCode", "RollPaper", "RollWidth" },
                values: new object[,]
                {
                    { 2, null, 30m, 0m, 0, 0, new DateTime(2022, 1, 19, 10, 29, 11, 958, DateTimeKind.Utc).AddTicks(9393), "natural white smooth matte surface", 190, true, "Ilford Galerie Graphic Heavyweight Matt - 17 roll", "IGXHWMP", true, 432 },
                    { 3, null, 35m, 0m, 0, 0, new DateTime(2022, 1, 19, 10, 29, 11, 958, DateTimeKind.Utc).AddTicks(9840), "natural white lightly pearled surface", 290, true, "Ilford Galerie Smooth Pearl - 17 roll", "IGSPP11", true, 432 },
                    { 4, null, 30m, 0m, 0, 0, new DateTime(2022, 1, 19, 10, 29, 11, 958, DateTimeKind.Utc).AddTicks(9845), "ultra white smooth matte surface", 230, true, "Olmec Photo Matt Archival - 17 roll", "OLM67R17", true, 432 },
                    { 5, null, 45m, 0m, 0, 0, new DateTime(2022, 1, 19, 10, 29, 11, 958, DateTimeKind.Utc).AddTicks(9847), "natural white smooth matte surface, grey backing for added opacity, easy-peel adhesive", 120, true, "Matt Self-adhesive Poly-vinyl - 17 roll", "M120V17", true, 432 },
                    { 6, null, 0m, 12m, 420, 297, new DateTime(2022, 1, 19, 10, 29, 11, 958, DateTimeKind.Utc).AddTicks(9850), "neutral white smooth lustre mettalic surface", 260, true, "Olmec Photo Metallic Lustre A3", "OLM72A3", false, 0 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b74ddd14-6340-4840-95c2-db12554843e5",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "ab02daac-7bfc-4fef-a145-180a125550bf", "df8ffe13-1b7c-4399-985d-a793c68118d9" });

            migrationBuilder.UpdateData(
                table: "Papers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CostPerMeter", "CostPerSheet", "CutSheetHeight", "CutSheetWidth", "DateCreated", "Description", "GSMWeight", "IsActive", "Name", "ProductCode", "RollPaper", "RollWidth" },
                values: new object[] { 0m, 10m, 420, 297, new DateTime(2022, 1, 18, 14, 43, 35, 502, DateTimeKind.Utc).AddTicks(429), "neutral white smooth lustre mettalic surface", 260, false, "Olmec Photo Metallic Lustre A3", null, false, 0 });
        }
    }
}
