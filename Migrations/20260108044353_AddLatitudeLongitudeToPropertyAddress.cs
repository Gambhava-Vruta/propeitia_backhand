using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Propertia.Migrations
{
    /// <inheritdoc />
    public partial class AddLatitudeLongitudeToPropertyAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "PropertyAddresses",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "PropertyAddresses",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "PropertyAddresses");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "PropertyAddresses");
        }
    }
}
