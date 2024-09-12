using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sales.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DataBasePopulationTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "CategoryId", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 1, "carnes-bovinas.jpg", "Carnes Bovinas" },
                    { 2, "produtos-diversos.jpg", "Produtos Diversos" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserId", "Cpf", "DateBirth", "Email", "Name", "Password", "Role" },
                values: new object[,]
                {
                    { 1, "111.111.111-11", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "migueltotti2005@gmail.com", "Miguel Totti de Oliveira", "testemiguel", 2 },
                    { 2, "222.222.222-22", new DateTime(2, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "isadorapaludeto15@gmail.com", "Isadora Leao Paludeto", "testeisadora", 2 },
                    { 31, "331.331.331-31", new DateTime(3, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "testeadmin@gmail.com", "TesteAdmin", "testeadmin", 2 },
                    { 32, "332.332.332-32", new DateTime(3, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "testeemployee@gmail.com", "TesteEmployee", "testeemployee", 1 },
                    { 33, "333.333.333-33", new DateTime(3, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "testecustomer@gmail.com", "TesteCustomer", "testecustomer", 0 }
                });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "ProductId", "CategoryId", "Description", "ImageUrl", "Name", "StockQuantity", "TypeValue", "Value" },
                values: new object[,]
                {
                    { 1, 2, "Coca Cola 250ml garrafinha", "coca-cola-250.jpg", "Coca-Cola 250", 10, 2, 3.5m },
                    { 2, 2, "Pão Caseiro feito no dia", "pao-caseiro.jpg", "Pão Caseiro", 3, 2, 9.9m },
                    { 3, 1, "Picanha", "picanha.jpg", "Picanha", 5, 1, 69.99m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "CategoryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Category",
                keyColumn: "CategoryId",
                keyValue: 2);
        }
    }
}
