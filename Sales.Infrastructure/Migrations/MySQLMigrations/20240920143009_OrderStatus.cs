using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sales.Infrastructure.Migrations.MySQLMigrations
{
    /// <inheritdoc />
    public partial class OrderStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderStatus",
                table: "Order",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Category",
                type: "varchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "CategoryId", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 1, "carnes-bovinas.jpg", "Carnes Bovinas" },
                    { 2, "produtos-diversos.jpg",  "Produtos Diversos" },
                    { 3, "aves.jpg", "Aves" },
                    { 4, "carnes-suinas.jpg", "Carnes Suinas" }
                });

            migrationBuilder.InsertData(
                table: "Order",
                columns: new[] { "OrderId", "OrderDate", "OrderStatus", "TotalValue", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 9, 19, 15, 50, 45, 0, DateTimeKind.Unspecified), 2, 10.00m, 1 },
                    { 2, new DateTime(2024, 9, 20, 15, 50, 45, 0, DateTimeKind.Unspecified), 1, 20.00m, 2 },
                    { 3, new DateTime(2024, 9, 19, 15, 51, 39, 0, DateTimeKind.Unspecified), 1, 30.00m, 1 },
                    { 4, new DateTime(2024, 9, 19, 15, 53, 36, 0, DateTimeKind.Unspecified), 2, 40.00m, 2 }
                });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "ProductId", "CategoryId", "Description", "ImageUrl", "Name", "StockQuantity", "TypeValue", "Value" },
                values: new object[,]
                {
                    { 1, 2, "Coca Cola 250ml garrafinha", "coca-cola-250.jpg", "Coca-Cola 250", 10, 2, 3.5m },
                    { 2, 2, "Pão Caseiro feito no dia", "pao-caseiro.jpg", "Pão Caseiro", 3, 2, 9.9m },
                    { 3, 1, "Picanha", "picanha.jpg", "Picanha", 5, 1, 69.99m },
                    { 4, 2, "TesteProduto", "TesteProduto.jpg", "Teste Produto", 10, 1, 10.00m },
                    { 5, 3, "Teste2Produto", "Teste2.jpg", "Teste2", 10, 2, 69.99m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "CategoryId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "CategoryId",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "Order");

            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Category",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(250)",
                oldMaxLength: 250,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
