using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sales.Infrastructure.Migrations.MergeDb
{
    /// <inheritdoc />
    public partial class DefaultDataAjust : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8a19b5bc-91ed-4399-8953-046eb2e1de37",
                columns: new[] { "ConcurrencyStamp", "NormalizedName" },
                values: new object[] { "b626c02b-4235-40b2-893c-d76a350746e7", "CUSTOMER" });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a5d55a1d-a654-452d-a24a-6f69985c11e3",
                columns: new[] { "ConcurrencyStamp", "NormalizedName" },
                values: new object[] { "16bca873-8f31-4cb4-ba36-e614c7ac4aa1", "ADMIN" });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b890f0aa-7486-4aa9-ba41-c76609a76476",
                columns: new[] { "ConcurrencyStamp", "NormalizedName" },
                values: new object[] { "b4e347da-0a49-4c41-9fd9-57894a766584", "EMPLOYEE" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshToken", "RefreshTokenExpiryTime", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "4de2810f-b57a-4aa0-b364-057c809160f9", 0, "19947c07-0772-45e5-8bce-7014b9ad8ac3", "admin@gmail.com", false, true, null, "ADMIN@GMAIL.COM", "ADMIN-DMAAMM", "AQAAAAIAAYagAAAAEEyRyv+ur5EUUt/0XE1Ptn12KryCQTpV1UEtn6sghOSd7bvnKIEPGUv94bMFvsIVeg==", null, false, null, new DateTime(2025, 4, 29, 21, 6, 58, 149, DateTimeKind.Local).AddTicks(3992), "ee16ee52-93fe-4acf-8bb9-317cfadf63a9", false, "Admin-dmaamm" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "Name", "Password" },
                values: new object[] { "Admin", "dHD+oA/Wkqs3YJ4JdWblRNFixjj8A2b2R4d+K2GNfKGhr7i56EwQ2YgFYcdbTAXFwnYEyjFjloYhCYcdiBJZOZpy/Q99ZDmk/fHTGOl3oTgQluSsV00wDwth1xaqVOsiuzG9YyNKeL1VdFTT1BW++Y3k3SxhC/niNC4od384zEU=" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "a5d55a1d-a654-452d-a24a-6f69985c11e3", "4de2810f-b57a-4aa0-b364-057c809160f9" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a5d55a1d-a654-452d-a24a-6f69985c11e3", "4de2810f-b57a-4aa0-b364-057c809160f9" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4de2810f-b57a-4aa0-b364-057c809160f9");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8a19b5bc-91ed-4399-8953-046eb2e1de37",
                columns: new[] { "ConcurrencyStamp", "NormalizedName" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a5d55a1d-a654-452d-a24a-6f69985c11e3",
                columns: new[] { "ConcurrencyStamp", "NormalizedName" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b890f0aa-7486-4aa9-ba41-c76609a76476",
                columns: new[] { "ConcurrencyStamp", "NormalizedName" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "Name", "Password" },
                values: new object[] { "admin", "Z8L6b+2GT7wDLhVNt84f61Ubb1L15ovQZMV4FNEsFCNxUOqpuqMuuGuSaIw83AhKgLN3+jokPTBeutx8kyCny4miTf2AfDZq+7OGLIS4paIP+ehC7pnRBDHSRhXMVwmXsH/17hWjfyrC+pGdqLze/snvMsZIulVIMNl1C/bNf2s=" });
        }
    }
}
