using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airline1.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckInAndBoardingPass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CheckIns",
                columns: table => new
                {
                    CheckInId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PassengerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SeatNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    QRCode = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckIns", x => x.CheckInId);
                    table.ForeignKey(
                        name: "FK_CheckIns_BookingPassengers_PassengerId",
                        column: x => x.PassengerId,
                        principalTable: "BookingPassengers",
                        principalColumn: "BookingPassengerId");
                    table.ForeignKey(
                        name: "FK_CheckIns_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BoardingPasses",
                columns: table => new
                {
                    BoardingPassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CheckInId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingReference = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    FlightNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PassengerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FromAirport = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ToAirport = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    DepartureDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SeatNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    QRCodeImageBase64 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BoardingPassUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardingPasses", x => x.BoardingPassId);
                    table.ForeignKey(
                        name: "FK_BoardingPasses_CheckIns_CheckInId",
                        column: x => x.CheckInId,
                        principalTable: "CheckIns",
                        principalColumn: "CheckInId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoardingPasses_BookingReference",
                table: "BoardingPasses",
                column: "BookingReference");

            migrationBuilder.CreateIndex(
                name: "IX_BoardingPasses_CheckInId",
                table: "BoardingPasses",
                column: "CheckInId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CheckIn_CreatedAt",
                table: "CheckIns",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CheckIns_BookingId",
                table: "CheckIns",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckIns_PassengerId",
                table: "CheckIns",
                column: "PassengerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoardingPasses");

            migrationBuilder.DropTable(
                name: "CheckIns");
        }
    }
}
