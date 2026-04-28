using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airline1.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatSale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SeatSaleConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaleEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SaleTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SaleSubtitle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    HeroMessage = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Version = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatSaleConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeatSales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DestinationAirportCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CountryIso2 = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Image = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OriginalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SalePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Discount = table.Column<int>(type: "int", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    PriceNote = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TravelPeriodStart = table.Column<DateOnly>(type: "date", nullable: false),
                    TravelPeriodEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    BookingDeadline = table.Column<DateOnly>(type: "date", nullable: false),
                    SeatsLeft = table.Column<int>(type: "int", nullable: false),
                    TotalSeats = table.Column<int>(type: "int", nullable: false),
                    Features = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Featured = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeatSales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TermsConditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeatSaleConfigId = table.Column<int>(type: "int", nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TermsConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TermsConditions_SeatSaleConfigs_SeatSaleConfigId",
                        column: x => x.SeatSaleConfigId,
                        principalTable: "SeatSaleConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeatSales_BookingDeadline",
                table: "SeatSales",
                column: "BookingDeadline");

            migrationBuilder.CreateIndex(
                name: "IX_SeatSales_DestinationAirportCode",
                table: "SeatSales",
                column: "DestinationAirportCode");

            migrationBuilder.CreateIndex(
                name: "IX_SeatSales_Featured",
                table: "SeatSales",
                column: "Featured");

            migrationBuilder.CreateIndex(
                name: "IX_SeatSales_IsActive",
                table: "SeatSales",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SeatSales_Type",
                table: "SeatSales",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_TermsConditions_SeatSaleConfigId",
                table: "TermsConditions",
                column: "SeatSaleConfigId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeatSales");

            migrationBuilder.DropTable(
                name: "TermsConditions");

            migrationBuilder.DropTable(
                name: "SeatSaleConfigs");
        }
    }
}
