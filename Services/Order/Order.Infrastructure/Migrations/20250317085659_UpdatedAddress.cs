using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_Line",
                schema: "order",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "Address_Province",
                schema: "order",
                table: "Orders",
                newName: "Address_City");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address_City",
                schema: "order",
                table: "Orders",
                newName: "Address_Province");

            migrationBuilder.AddColumn<string>(
                name: "Address_Line",
                schema: "order",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
