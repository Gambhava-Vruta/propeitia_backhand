using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Propertia.Migrations
{
    /// <inheritdoc />
    public partial class FixPropertyPricePrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PropertyPriceId",
                table: "PropertyPrices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PropertyPriceId",
                table: "PropertyPrices");
        }
    }
}
