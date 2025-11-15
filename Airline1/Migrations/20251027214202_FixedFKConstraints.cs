using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airline1.Migrations
{
    /// <inheritdoc />
    public partial class FixedFKConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    FlightId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Flights_FlightId",
                        column: x => x.FlightId,
                        principalTable: "Flights",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BookingPassengers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    FlightId = table.Column<int>(type: "int", nullable: false),
                    PassengerId = table.Column<int>(type: "int", nullable: true),
                    PassengerName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PassengerEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SeatNumber = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    IsContinuingPassenger = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingPassengers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingPassengers_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingPassengers_Passengers_PassengerId",
                        column: x => x.PassengerId,
                        principalTable: "Passengers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_BookingId",
                table: "Passengers",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Aircrafts_ConfigurationID",
                table: "Aircrafts",
                column: "ConfigurationID");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPassengers_BookingId",
                table: "BookingPassengers",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPassengers_FlightId_SeatNumber",
                table: "BookingPassengers",
                columns: new[] { "FlightId", "SeatNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingPassengers_PassengerId",
                table: "BookingPassengers",
                column: "PassengerId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingCode",
                table: "Bookings",
                column: "BookingCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_FlightId",
                table: "Bookings",
                column: "FlightId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Aircrafts_AircraftConfigurations_ConfigurationID",
                table: "Aircrafts",
                column: "ConfigurationID",
                principalTable: "AircraftConfigurations",
                principalColumn: "ConfigurationID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Passengers_Bookings_BookingId",
                table: "Passengers",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Aircrafts_AircraftConfigurations_ConfigurationID",
                table: "Aircrafts");

            migrationBuilder.DropForeignKey(
                name: "FK_Passengers_Bookings_BookingId",
                table: "Passengers");

            migrationBuilder.DropTable(
                name: "BookingPassengers");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Passengers_BookingId",
                table: "Passengers");

            migrationBuilder.DropIndex(
                name: "IX_Aircrafts_ConfigurationID",
                table: "Aircrafts");
        }
    }
}
