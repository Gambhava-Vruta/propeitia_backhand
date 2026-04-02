using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Propertia.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressExtraFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FamousArea",
                table: "PropertyAddresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Landmark",
                table: "PropertyAddresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SocietyName",
                table: "PropertyAddresses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FamousArea",
                table: "PropertyAddresses");

            migrationBuilder.DropColumn(
                name: "Landmark",
                table: "PropertyAddresses");

            migrationBuilder.DropColumn(
                name: "SocietyName",
                table: "PropertyAddresses");
        }
    }
}
