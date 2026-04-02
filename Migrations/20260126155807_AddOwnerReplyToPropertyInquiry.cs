using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Propertia.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerReplyToPropertyInquiry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerReply",
                table: "PropertyInquiries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReplyDate",
                table: "PropertyInquiries",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerReply",
                table: "PropertyInquiries");

            migrationBuilder.DropColumn(
                name: "ReplyDate",
                table: "PropertyInquiries");
        }
    }
}
