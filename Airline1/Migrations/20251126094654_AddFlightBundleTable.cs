using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airline1.Migrations
{
    /// <inheritdoc />
    public partial class AddFlightBundleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlightBundles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PriceIncrement = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CarryOnWeightKg = table.Column<int>(type: "int", nullable: false),
                    CheckedBaggagePcs = table.Column<int>(type: "int", nullable: false),
                    CheckedBaggageWeightKg = table.Column<int>(type: "int", nullable: false),
                    IncludesPreferredSeatSelection = table.Column<bool>(type: "bit", nullable: false),
                    ChangeFeeType = table.Column<int>(type: "int", nullable: false),
                    IsCancellable = table.Column<bool>(type: "bit", nullable: false),
                    AllowsTravelFundConversion = table.Column<bool>(type: "bit", nullable: false),
                    Tagline = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightBundles", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightBundles");
        }
    }
}
