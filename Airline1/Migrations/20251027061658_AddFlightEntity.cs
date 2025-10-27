using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airline1.Migrations
{
    /// <inheritdoc />
    public partial class AddFlightEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeatingCapacity",
                table: "Aircrafts");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Aircrafts");

            migrationBuilder.AddColumn<string>(
                name: "ConfigurationID",
                table: "Aircrafts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfigurationID",
                table: "Aircrafts");

            migrationBuilder.AddColumn<int>(
                name: "SeatingCapacity",
                table: "Aircrafts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Aircrafts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
