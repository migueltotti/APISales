using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sales.Infrastructure.Migrations.MergeDb
{
    /// <inheritdoc />
    public partial class DefaultData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Affiliate",
                keyColumn: "AffiliateId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "LineItem",
                keyColumn: "LineItemId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LineItem",
                keyColumn: "LineItemId",
                keyValue: 2);

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
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "ProductId",
                keyValue: 5);

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

            migrationBuilder.DeleteData(
                table: "Order",
                keyColumn: "OrderId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Affiliate",
                keyColumn: "AffiliateId",
                keyValue: 1,
                column: "Name",
                value: "Nenhuma Afiliação");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8a19b5bc-91ed-4399-8953-046eb2e1de37", null, "Customer", null },
                    { "a5d55a1d-a654-452d-a24a-6f69985c11e3", null, "Admin", null },
                    { "b890f0aa-7486-4aa9-ba41-c76609a76476", null, "Employee", null }
                });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "CategoryId",
                keyValue: 1,
                columns: new[] { "ImageUrl", "Name" },
                values: new object[] { "bovinos.jpg", "Bonivos" });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "CategoryId",
                keyValue: 2,
                columns: new[] { "ImageUrl", "Name" },
                values: new object[] { "suinos.jpg", "Suínos" });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "CategoryId",
                keyValue: 4,
                columns: new[] { "ImageUrl", "Name" },
                values: new object[] { "diversos.jpg", "Diversos" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "Cpf", "Email", "Name", "Password" },
                values: new object[] { "000.000.000-00", "admin@gmail.com", "admin", "Z8L6b+2GT7wDLhVNt84f61Ubb1L15ovQZMV4FNEsFCNxUOqpuqMuuGuSaIw83AhKgLN3+jokPTBeutx8kyCny4miTf2AfDZq+7OGLIS4paIP+ehC7pnRBDHSRhXMVwmXsH/17hWjfyrC+pGdqLze/snvMsZIulVIMNl1C/bNf2s=" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8a19b5bc-91ed-4399-8953-046eb2e1de37");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a5d55a1d-a654-452d-a24a-6f69985c11e3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b890f0aa-7486-4aa9-ba41-c76609a76476");

            migrationBuilder.UpdateData(
                table: "Affiliate",
                keyColumn: "AffiliateId",
                keyValue: 1,
                column: "Name",
                value: "Nenhuma Afiliacao");

            migrationBuilder.InsertData(
                table: "Affiliate",
                columns: new[] { "AffiliateId", "Discount", "Name" },
                values: new object[,]
                {
                    { 3, 5.00m, "Duratex" },
                    { 4, 10.00m, "Teste" }
                });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "CategoryId",
                keyValue: 1,
                columns: new[] { "ImageUrl", "Name" },
                values: new object[] { "carnes-bovinas.jpg", "Carnes Bovinas" });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "CategoryId",
                keyValue: 2,
                columns: new[] { "ImageUrl", "Name" },
                values: new object[] { "produtos-diversos.jpg", "Produtos Diversos" });

            migrationBuilder.UpdateData(
                table: "Category",
                keyColumn: "CategoryId",
                keyValue: 4,
                columns: new[] { "ImageUrl", "Name" },
                values: new object[] { "carnes-suinas.jpg", "Carnes Suinas" });

            migrationBuilder.InsertData(
                table: "Order",
                columns: new[] { "OrderId", "Holder", "Note", "OrderDate", "OrderStatus", "TotalValue", "UserId" },
                values: new object[,]
                {
                    { 1, "", "", new DateTime(2024, 9, 19, 15, 50, 45, 0, DateTimeKind.Unspecified), 3, 10.00m, 1 },
                    { 3, "", "", new DateTime(2024, 9, 19, 15, 51, 39, 0, DateTimeKind.Unspecified), 1, 30.00m, 1 },
                    { 7, "Miguel Totti", null, new DateTime(2024, 9, 30, 8, 33, 16, 0, DateTimeKind.Unspecified), 3, 83.49m, null }
                });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "ProductId", "CategoryId", "Description", "ImageUrl", "Name", "StockQuantity", "TypeValue", "Value" },
                values: new object[,]
                {
                    { 4, 2, "TesteProduto", "TesteProduto.jpg", "Teste Produto", 10, 1, 10.00m },
                    { 5, 3, "Teste2Produto", "Teste2.jpg", "Teste2", 10, 2, 69.99m }
                });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "Cpf", "Email", "Name", "Password" },
                values: new object[] { "111.111.111-11", "migueltotti2005@gmail.com", "Miguel Totti de Oliveira", "testemiguel" });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserId", "AffiliateId", "Cpf", "DateBirth", "Email", "Name", "Password", "Points", "Role" },
                values: new object[,]
                {
                    { 2, 1, "222.222.222-22", new DateTime(2, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "isadorapaludeto15@gmail.com", "Isadora Leao Paludeto", "testeisadora", 0.00m, 2 },
                    { 31, 1, "331.331.331-31", new DateTime(3, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "testeadmin@gmail.com", "TesteAdmin", "testeadmin", 0.00m, 2 },
                    { 32, 1, "332.332.332-32", new DateTime(3, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "testeemployee@gmail.com", "TesteEmployee", "testeemployee", 0.00m, 1 },
                    { 33, 1, "333.333.333-33", new DateTime(3, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "testecustomer@gmail.com", "TesteCustomer", "testecustomer", 0.00m, 0 },
                    { 36, 1, "777.777.777-77", new DateTime(2024, 9, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "TesteUsuarioToken@gmail.com", "TesteUsuarioToken", "TesteToken1234@", 0.00m, 0 },
                    { 38, 1, "890.123.434-22", new DateTime(2024, 9, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "testedeusercomrole@gmail.com", "Teste de User com Role", "Testeusserrole1@", 0.00m, 1 }
                });

            migrationBuilder.InsertData(
                table: "LineItem",
                columns: new[] { "LineItemId", "Amount", "OrderId", "Price", "ProductId" },
                values: new object[,]
                {
                    { 1, 3m, 1, 3.5m, 1 },
                    { 2, 1m, 1, 9.9m, 2 }
                });

            migrationBuilder.InsertData(
                table: "Order",
                columns: new[] { "OrderId", "Holder", "Note", "OrderDate", "OrderStatus", "TotalValue", "UserId" },
                values: new object[,]
                {
                    { 2, "", "Sem tomate", new DateTime(2024, 9, 20, 15, 50, 45, 0, DateTimeKind.Unspecified), 2, 20.00m, 2 },
                    { 4, "", "Cortado em tiras", new DateTime(2024, 9, 19, 15, 53, 36, 0, DateTimeKind.Unspecified), 3, 40.00m, 2 },
                    { 5, "", "", new DateTime(2024, 9, 20, 17, 47, 58, 0, DateTimeKind.Unspecified), 1, 0.00m, 2 },
                    { 6, "", "Duplo", new DateTime(2024, 9, 30, 8, 33, 16, 0, DateTimeKind.Unspecified), 3, 83.49m, 2 }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserId", "AffiliateId", "Cpf", "DateBirth", "Email", "Name", "Password", "Points", "Role" },
                values: new object[] { 35, 3, "444.444.444-44", new DateTime(2024, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "TESTEAFILIACAO@gmail.com", "TESTEAFILIACAO", "TESTEAFaaaLIACAO213123@#@#", 0.00m, 0 });
        }
    }
}
