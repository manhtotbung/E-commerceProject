using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBH.Data.Migrations
{
    /// <inheritdoc />
    public partial class changeVariantProductCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Carts_ProductId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_VariantId",
                table: "Carts");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_ProductId",
                table: "Carts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_VariantId",
                table: "Carts",
                column: "VariantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Carts_ProductId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_VariantId",
                table: "Carts");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_ProductId",
                table: "Carts",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_VariantId",
                table: "Carts",
                column: "VariantId",
                unique: true,
                filter: "[VariantId] IS NOT NULL");
        }
    }
}
