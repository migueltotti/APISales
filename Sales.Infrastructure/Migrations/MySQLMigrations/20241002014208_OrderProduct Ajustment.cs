using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sales.Infrastructure.Migrations.MySQLMigrations
{
    /// <inheritdoc />
    public partial class OrderProductAjustment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.AddColumn<decimal>(
                name: "ProductAmount",
                table: "OrderProduct",
                type: "decimal(6,2)",
                precision: 6,
                scale: 2,
                nullable: false,
                defaultValue: 0m);*/

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 1,
                column: "OrderStatus",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 2,
                column: "OrderStatus",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 4,
                column: "OrderStatus",
                value: 3);

            /*migrationBuilder.InsertData(
                table: "Order",
                columns: new[] { "OrderId", "OrderDate", "OrderStatus", "TotalValue", "UserId" },
                values: new object[,]
                {
                    { 5, new DateTime(2024, 9, 20, 17, 47, 58, 0, DateTimeKind.Unspecified), 1, 0.00m, 2 },
                    { 6, new DateTime(2024, 9, 30, 8, 33, 16, 0, DateTimeKind.Unspecified), 3, 83.49m, 2 }
                });*/

            /*migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserId", "AffiliateId", "Cpf", "DateBirth", "Email", "Name", "Password", "Points", "Role" },
                values: new object[,]
                {
                    { 36, 1, "777.777.777-77", new DateTime(2024, 9, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "TesteUsuarioToken@gmail.com", "TesteUsuarioToken", "TesteToken1234@", 0.00m, 0 },
                    { 38, 1, "890.123.434-22", new DateTime(2024, 9, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "testedeusercomrole@gmail.com", "Teste de User com Role", "Testeusserrole1@", 0.00m, 1 },
                    { 35, 3, "444.444.444-44", new DateTime(2024, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "TESTEAFILIACAO@gmail.com", "TESTEAFILIACAO", "TESTEAFaaaLIACAO213123@#@#", 0.00m, 0 }
                });*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Affiliate",
                keyColumn: "AffiliateId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Affiliate",
                keyColumn: "AffiliateId",
                keyValue: 3);

            migrationBuilder.AddColumn<decimal>(
                name: "ProductAmount",
                table: "OrderProduct",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 1,
                column: "OrderStatus",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 2,
                column: "OrderStatus",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 4,
                column: "OrderStatus",
                value: 2);
        }
    }
}
