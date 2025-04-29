using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sales.Infrastructure.Migrations.TesteMigrations
{
    /// <inheritdoc />
    public partial class InitalTestMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Affiliate",
                columns: table => new
                {
                    AffiliateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Discount = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Affiliate", x => x.AffiliateId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ImageUrl = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(400)", maxLength: 400, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cpf = table.Column<string>(type: "varchar(14)", maxLength: 14, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Points = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: false),
                    DateBirth = table.Column<DateTime>(type: "date", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    AffiliateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_User_Affiliate_AffiliateId",
                        column: x => x.AffiliateId,
                        principalTable: "Affiliate",
                        principalColumn: "AffiliateId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(175)", maxLength: 175, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    TypeValue = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StockQuantity = table.Column<int>(type: "int", maxLength: 80, nullable: false, defaultValue: 0),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Product_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TotalValue = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    OrderStatus = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Order_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ShoppingCart",
                columns: table => new
                {
                    ShoppingCartId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TotalValue = table.Column<double>(type: "double", precision: 10, scale: 2, nullable: false),
                    ProductsCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCart", x => x.ShoppingCartId);
                    table.ForeignKey(
                        name: "FK_ShoppingCart_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LineItem",
                columns: table => new
                {
                    LineItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(8,3)", precision: 8, scale: 3, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineItem", x => x.LineItemId);
                    table.ForeignKey(
                        name: "FK_LineItem_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LineItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ShoppingCartProducts",
                columns: table => new
                {
                    ShoppingCartId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Checked = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    Amount = table.Column<decimal>(type: "decimal(10,3)", precision: 10, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCartProducts", x => new { x.ProductId, x.ShoppingCartId });
                    table.ForeignKey(
                        name: "FK_ShoppingCartProducts_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShoppingCartProducts_ShoppingCart_ShoppingCartId",
                        column: x => x.ShoppingCartId,
                        principalTable: "ShoppingCart",
                        principalColumn: "ShoppingCartId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Affiliate",
                columns: new[] { "AffiliateId", "Discount", "Name" },
                values: new object[,]
                {
                    { 1, 0.00m, "Nenhuma Afiliacao" },
                    { 3, 5.00m, "Duratex" },
                    { 4, 10.00m, "Teste" }
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "CategoryId", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 1, "carnes-bovinas.jpg", "Carnes Bovinas" },
                    { 2, "produtos-diversos.jpg", "Produtos Diversos" },
                    { 3, "aves.jpg", "Aves" },
                    { 4, "carnes-suinas.jpg", "Carnes Suinas" }
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

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserId", "AffiliateId", "Cpf", "DateBirth", "Email", "Name", "Password", "Points", "Role" },
                values: new object[,]
                {
                    { 1, 1, "111.111.111-11", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "migueltotti2005@gmail.com", "Miguel Totti de Oliveira", "testemiguel", 0.00m, 2 },
                    { 2, 1, "222.222.222-22", new DateTime(2, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "isadorapaludeto15@gmail.com", "Isadora Leao Paludeto", "testeisadora", 0.00m, 2 },
                    { 31, 1, "331.331.331-31", new DateTime(3, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "testeadmin@gmail.com", "TesteAdmin", "testeadmin", 0.00m, 2 },
                    { 32, 1, "332.332.332-32", new DateTime(3, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "testeemployee@gmail.com", "TesteEmployee", "testeemployee", 0.00m, 1 },
                    { 33, 1, "333.333.333-33", new DateTime(3, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "testecustomer@gmail.com", "TesteCustomer", "testecustomer", 0.00m, 0 },
                    { 35, 3, "444.444.444-44", new DateTime(2024, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "TESTEAFILIACAO@gmail.com", "TESTEAFILIACAO", "TESTEAFaaaLIACAO213123@#@#", 0.00m, 0 },
                    { 36, 1, "777.777.777-77", new DateTime(2024, 9, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "TesteUsuarioToken@gmail.com", "TesteUsuarioToken", "TesteToken1234@", 0.00m, 0 },
                    { 38, 1, "890.123.434-22", new DateTime(2024, 9, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "testedeusercomrole@gmail.com", "Teste de User com Role", "Testeusserrole1@", 0.00m, 1 }
                });

            migrationBuilder.InsertData(
                table: "Order",
                columns: new[] { "OrderId", "Note", "OrderDate", "OrderStatus", "TotalValue", "UserId" },
                values: new object[,]
                {
                    { 1, "", new DateTime(2024, 9, 19, 15, 50, 45, 0, DateTimeKind.Unspecified), 3, 10.00m, 1 },
                    { 2, "Sem tomate", new DateTime(2024, 9, 20, 15, 50, 45, 0, DateTimeKind.Unspecified), 2, 20.00m, 2 },
                    { 3, "", new DateTime(2024, 9, 19, 15, 51, 39, 0, DateTimeKind.Unspecified), 1, 30.00m, 1 },
                    { 4, "Cortado em tiras", new DateTime(2024, 9, 19, 15, 53, 36, 0, DateTimeKind.Unspecified), 3, 40.00m, 2 },
                    { 5, "", new DateTime(2024, 9, 20, 17, 47, 58, 0, DateTimeKind.Unspecified), 1, 0.00m, 2 },
                    { 6, "Duplo", new DateTime(2024, 9, 30, 8, 33, 16, 0, DateTimeKind.Unspecified), 3, 83.49m, 2 }
                });

            migrationBuilder.InsertData(
                table: "LineItem",
                columns: new[] { "LineItemId", "Amount", "OrderId", "Price", "ProductId" },
                values: new object[,]
                {
                    { 1, 3m, 1, 3.5m, 1 },
                    { 2, 1m, 1, 9.9m, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_LineItem_OrderId",
                table: "LineItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_LineItem_ProductId",
                table: "LineItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_UserId",
                table: "Order",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CategoryId",
                table: "Product",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCart_UserId",
                table: "ShoppingCart",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartProducts_ShoppingCartId",
                table: "ShoppingCartProducts",
                column: "ShoppingCartId");

            migrationBuilder.CreateIndex(
                name: "IX_User_AffiliateId",
                table: "User",
                column: "AffiliateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineItem");

            migrationBuilder.DropTable(
                name: "ShoppingCartProducts");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "ShoppingCart");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Affiliate");
        }
    }
}
