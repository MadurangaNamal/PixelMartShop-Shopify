using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PixelMartShop.Migrations
{
    /// <inheritdoc />
    public partial class update_productVarient_options : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Option1",
                table: "ProductVariants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Option2",
                table: "ProductVariants",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Option1",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "Option2",
                table: "ProductVariants");
        }
    }
}
