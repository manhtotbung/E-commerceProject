using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBH.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldColorinProductVariants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
         
            migrationBuilder.RenameColumn(
                name: "Color",
                table: "ProductVarient",
                newName: "ColorName");

            migrationBuilder.AddColumn<string>(
                name: "ColorId",
                table: "ProductVarient",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "ProductVarient");

            migrationBuilder.RenameColumn(
                name: "ColorName",
                table: "ProductVarient",
                newName: "Color");

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
