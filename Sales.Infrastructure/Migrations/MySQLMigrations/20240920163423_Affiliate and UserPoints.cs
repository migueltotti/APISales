using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sales.Infrastructure.Migrations.MySQLMigrations
{
    /// <inheritdoc />
    public partial class AffiliateandUserPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AffiliateId",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Points",
                table: "User",
                type: "decimal(6,2)",
                precision: 6,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

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

            migrationBuilder.InsertData(
                table: "Affiliate",
                columns: new[] { "AffiliateId", "Discount", "Name" },
                values: new object[] { 1, 0.00m, "Nenhuma Afiliacao" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "AffiliateId", "Points" },
                values: new object[] { 1, 0.00m });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "AffiliateId", "Points" },
                values: new object[] { 1, 0.00m });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 31,
                columns: new[] { "AffiliateId", "Points" },
                values: new object[] { 1, 0.00m });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 32,
                columns: new[] { "AffiliateId", "Points" },
                values: new object[] { 1, 0.00m });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 33,
                columns: new[] { "AffiliateId", "Points" },
                values: new object[] { 1, 0.00m });

            migrationBuilder.CreateIndex(
                name: "IX_User_AffiliateId",
                table: "User",
                column: "AffiliateId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Affiliate_AffiliateId",
                table: "User",
                column: "AffiliateId",
                principalTable: "Affiliate",
                principalColumn: "AffiliateId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Affiliate_AffiliateId",
                table: "User");

            migrationBuilder.DropTable(
                name: "Affiliate");

            migrationBuilder.DropIndex(
                name: "IX_User_AffiliateId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "AffiliateId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "User");
        }
    }
}
