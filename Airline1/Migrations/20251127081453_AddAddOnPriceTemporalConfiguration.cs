using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airline1.Migrations
{
    /// <inheritdoc />
    public partial class AddAddOnPriceTemporalConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddOnPrices_FlightAddOns_AddOnId",
                table: "AddOnPrices");

            migrationBuilder.DropIndex(
                name: "IX_AddOnPrices_FlightId",
                table: "AddOnPrices");

            migrationBuilder.CreateIndex(
                name: "IX_AddOnPrice_Flight_AddOn_Effective",
                table: "AddOnPrices",
                columns: new[] { "FlightId", "AddOnId", "ValidFrom" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AddOnPrices_FlightAddOns_AddOnId",
                table: "AddOnPrices",
                column: "AddOnId",
                principalTable: "FlightAddOns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AddOnPrices_FlightAddOns_AddOnId",
                table: "AddOnPrices");

            migrationBuilder.DropIndex(
                name: "IX_AddOnPrice_Flight_AddOn_Effective",
                table: "AddOnPrices");

            migrationBuilder.CreateIndex(
                name: "IX_AddOnPrices_FlightId",
                table: "AddOnPrices",
                column: "FlightId");

            migrationBuilder.AddForeignKey(
                name: "FK_AddOnPrices_FlightAddOns_AddOnId",
                table: "AddOnPrices",
                column: "AddOnId",
                principalTable: "FlightAddOns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
