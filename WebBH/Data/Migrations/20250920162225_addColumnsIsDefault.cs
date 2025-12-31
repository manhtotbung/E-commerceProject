using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBH.Data.Migrations
{
    /// <inheritdoc />
    public partial class addColumnsIsDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_ProductVarient_VariantId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ProductVarient_ProductVariantId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVarient_Products_ProductId",
                table: "ProductVarient");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductVarient",
                table: "ProductVarient");

            migrationBuilder.RenameTable(
                name: "ProductVarient",
                newName: "ProductVariants");

            migrationBuilder.RenameIndex(
                name: "IX_ProductVarient_ProductId",
                table: "ProductVariants",
                newName: "IX_ProductVariants_ProductId");

            migrationBuilder.AddColumn<int>(
                name: "VariantId",
                table: "CartItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "ProductVariants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductVariants",
                table: "ProductVariants",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_ProductVariants_VariantId",
                table: "Carts",
                column: "VariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ProductVariants_ProductVariantId",
                table: "OrderItems",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_Products_ProductId",
                table: "ProductVariants",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_ProductVariants_VariantId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_ProductVariants_ProductVariantId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_Products_ProductId",
                table: "ProductVariants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductVariants",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "VariantId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "ProductVariants");

            migrationBuilder.RenameTable(
                name: "ProductVariants",
                newName: "ProductVarient");

            migrationBuilder.RenameIndex(
                name: "IX_ProductVariants_ProductId",
                table: "ProductVarient",
                newName: "IX_ProductVarient_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductVarient",
                table: "ProductVarient",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_ProductVarient_VariantId",
                table: "Carts",
                column: "VariantId",
                principalTable: "ProductVarient",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_ProductVarient_ProductVariantId",
                table: "OrderItems",
                column: "ProductVariantId",
                principalTable: "ProductVarient",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVarient_Products_ProductId",
                table: "ProductVarient",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
