using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sales.Infrastructure.Migrations.MergeDb
{
    /// <inheritdoc />
    public partial class FixDefaultTableData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8a19b5bc-91ed-4399-8953-046eb2e1de37",
                column: "ConcurrencyStamp",
                value: "bf4a4cdf-108f-46b2-a153-f6b79f20a2da");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a5d55a1d-a654-452d-a24a-6f69985c11e3",
                column: "ConcurrencyStamp",
                value: "35fbed1d-f043-483f-99f2-b0b3ce7ebbb0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b890f0aa-7486-4aa9-ba41-c76609a76476",
                column: "ConcurrencyStamp",
                value: "f1cb4829-f5a5-4784-a752-f6aaefe91450");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4de2810f-b57a-4aa0-b364-057c809160f9",
                columns: new[] { "RefreshTokenExpiryTime", "SecurityStamp" },
                values: new object[] { new DateTime(2025, 5, 9, 18, 16, 41, 869, DateTimeKind.Local).AddTicks(2661), "ec7a621a-4889-4b59-ab5b-4ce5f64e6b39" });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "CategoryId",
                keyValue: 1,
                column: "Name",
                value: "Bovinos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8a19b5bc-91ed-4399-8953-046eb2e1de37",
                column: "ConcurrencyStamp",
                value: "b626c02b-4235-40b2-893c-d76a350746e7");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a5d55a1d-a654-452d-a24a-6f69985c11e3",
                column: "ConcurrencyStamp",
                value: "16bca873-8f31-4cb4-ba36-e614c7ac4aa1");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b890f0aa-7486-4aa9-ba41-c76609a76476",
                column: "ConcurrencyStamp",
                value: "b4e347da-0a49-4c41-9fd9-57894a766584");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4de2810f-b57a-4aa0-b364-057c809160f9",
                columns: new[] { "RefreshTokenExpiryTime", "SecurityStamp" },
                values: new object[] { new DateTime(2025, 4, 29, 21, 6, 58, 149, DateTimeKind.Local).AddTicks(3992), "ee16ee52-93fe-4acf-8bb9-317cfadf63a9" });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "CategoryId",
                keyValue: 1,
                column: "Name",
                value: "Bonivos");
        }
    }
}
