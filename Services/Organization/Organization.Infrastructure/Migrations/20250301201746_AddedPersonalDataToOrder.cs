using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organization.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedPersonalDataToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClosedBy",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpenedBy",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "aaffdd6d-38be-4cf8-8da9-e8d93b0ec150");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosedBy",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OpenedBy",
                table: "Orders");
        }
    }
}
