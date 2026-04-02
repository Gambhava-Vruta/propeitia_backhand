using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Propertia.Migrations
{
    /// <inheritdoc />
    public partial class FinalizePropertyPriceSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Remove FK created by shadow property
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyPrices_Properties_PropertyId1",
                table: "PropertyPrices");

            // 2. Remove index + column
            migrationBuilder.DropIndex(
                name: "IX_PropertyPrices_PropertyId1",
                table: "PropertyPrices");

            migrationBuilder.DropColumn(
                name: "PropertyId1",
                table: "PropertyPrices");

            // 3. Drop old primary key
            migrationBuilder.DropPrimaryKey(
                name: "PK_PropertyPrices",
                table: "PropertyPrices");

            // 4. Drop PropertyPriceId completely
            migrationBuilder.DropColumn(
                name: "PropertyPriceId",
                table: "PropertyPrices");

            // 5. Recreate PropertyPriceId WITH IDENTITY
            migrationBuilder.AddColumn<int>(
                name: "PropertyPriceId",
                table: "PropertyPrices",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            // 6. Add primary key
            migrationBuilder.AddPrimaryKey(
                name: "PK_PropertyPrices",
                table: "PropertyPrices",
                column: "PropertyPriceId");

            // 7. Add unique constraint (Rent / Sale)
            migrationBuilder.CreateIndex(
                name: "IX_PropertyPrices_PropertyId_TransactionTypeId",
                table: "PropertyPrices",
                columns: new[] { "PropertyId", "TransactionTypeId" },
                unique: true);
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PropertyPrices",
                table: "PropertyPrices");

            migrationBuilder.DropIndex(
                name: "IX_PropertyPrices_PropertyId_TransactionTypeId",
                table: "PropertyPrices");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyPriceId",
                table: "PropertyPrices",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "PropertyId1",
                table: "PropertyPrices",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PropertyPrices",
                table: "PropertyPrices",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyPrices_PropertyId1",
                table: "PropertyPrices",
                column: "PropertyId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyPrices_Properties_PropertyId1",
                table: "PropertyPrices",
                column: "PropertyId1",
                principalTable: "Properties",
                principalColumn: "PropertyId");
        }
    }
}
