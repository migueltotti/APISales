using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sales.Infrastructure.Migrations.PostGresMigrations
{
    /// <inheritdoc />
    public partial class UpdateUserNameAndEmailIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove o índice único em NormalizedUserName
            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            // Remove o índice não único em NormalizedEmail
            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "AspNetUsers");

            // Cria o índice não único para NormalizedUserName
            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName");

            // Cria o índice único para NormalizedEmail
            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverte para o estado anterior

            // Remove o índice não único em NormalizedUserName
            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            // Remove o índice único em NormalizedEmail
            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "AspNetUsers");

            // Restaura o índice único em NormalizedUserName
            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            // Restaura o índice não único em NormalizedEmail
            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");
        }
    }
}
