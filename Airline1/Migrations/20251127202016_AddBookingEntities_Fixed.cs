using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airline1.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingEntities_Fixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingPassengers_Passengers_PassengerId",
                table: "BookingPassengers");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_FlightPrice_Temporal_Bundle",
                table: "FlightPrices");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_BookingCode",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_BookingPassengers_FlightId_SeatNumber",
                table: "BookingPassengers");

            migrationBuilder.DropIndex(
                name: "IX_BookingPassengers_PassengerId",
                table: "BookingPassengers");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "FlightId",
                table: "BookingPassengers");

            migrationBuilder.DropColumn(
                name: "IsContinuingPassenger",
                table: "BookingPassengers");

            migrationBuilder.DropColumn(
                name: "PassengerEmail",
                table: "BookingPassengers");

            migrationBuilder.DropColumn(
                name: "PassengerName",
                table: "BookingPassengers");

            migrationBuilder.DropColumn(
                name: "SeatNumber",
                table: "BookingPassengers");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Bookings",
                newName: "BookingDate");

            migrationBuilder.RenameColumn(
                name: "BookingCode",
                table: "Bookings",
                newName: "ContactPhone");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Bookings",
                newName: "BookingId");

            migrationBuilder.RenameColumn(
                name: "PassengerId",
                table: "BookingPassengers",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "BookingPassengers",
                newName: "DateOfBirth");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BookingPassengers",
                newName: "BookingPassengerId");

            migrationBuilder.AddColumn<string>(
                name: "PassengerType",
                table: "FlightPrices",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Bookings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Bookings",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Bookings",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FlightBundleId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pnr",
                table: "Bookings",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "BookingPassengers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FlightSeatId",
                table: "BookingPassengers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "BookingPassengers",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "BookingPassengers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                table: "BookingPassengers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassengerType",
                table: "BookingPassengers",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "BookingAddOns",
                columns: table => new
                {
                    BookingAddOnId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PassengerId = table.Column<int>(type: "int", nullable: false),
                    AddOnPriceId = table.Column<int>(type: "int", nullable: false),
                    PriceAtBooking = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingAddOns", x => x.BookingAddOnId);
                    table.ForeignKey(
                        name: "FK_BookingAddOns_AddOnPrices_AddOnPriceId",
                        column: x => x.AddOnPriceId,
                        principalTable: "AddOnPrices",
                        principalColumn: "AddOnPriceId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BookingAddOns_BookingPassengers_PassengerId",
                        column: x => x.PassengerId,
                        principalTable: "BookingPassengers",
                        principalColumn: "BookingPassengerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlightPrice_Temporal_Bundle",
                table: "FlightPrices",
                columns: new[] { "FlightId", "CabinClass", "FlightBundleId", "PassengerType", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_FlightBundleId",
                table: "Bookings",
                column: "FlightBundleId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Pnr",
                table: "Bookings",
                column: "Pnr",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingPassengers_FlightSeatId",
                table: "BookingPassengers",
                column: "FlightSeatId",
                unique: true,
                filter: "[FlightSeatId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BookingAddOns_AddOnPriceId",
                table: "BookingAddOns",
                column: "AddOnPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingAddOns_PassengerId",
                table: "BookingAddOns",
                column: "PassengerId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingPassengers_FlightSeats_FlightSeatId",
                table: "BookingPassengers",
                column: "FlightSeatId",
                principalTable: "FlightSeats",
                principalColumn: "FlightSeatId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_FlightBundles_FlightBundleId",
                table: "Bookings",
                column: "FlightBundleId",
                principalTable: "FlightBundles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingPassengers_FlightSeats_FlightSeatId",
                table: "BookingPassengers");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_FlightBundles_FlightBundleId",
                table: "Bookings");

            migrationBuilder.DropTable(
                name: "BookingAddOns");

            migrationBuilder.DropIndex(
                name: "IX_FlightPrice_Temporal_Bundle",
                table: "FlightPrices");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_FlightBundleId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_Pnr",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_BookingPassengers_FlightSeatId",
                table: "BookingPassengers");

            migrationBuilder.DropColumn(
                name: "PassengerType",
                table: "FlightPrices");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "FlightBundleId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Pnr",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "BookingPassengers");

            migrationBuilder.DropColumn(
                name: "FlightSeatId",
                table: "BookingPassengers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "BookingPassengers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "BookingPassengers");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                table: "BookingPassengers");

            migrationBuilder.DropColumn(
                name: "PassengerType",
                table: "BookingPassengers");

            migrationBuilder.RenameColumn(
                name: "ContactPhone",
                table: "Bookings",
                newName: "BookingCode");

            migrationBuilder.RenameColumn(
                name: "BookingDate",
                table: "Bookings",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "BookingId",
                table: "Bookings",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BookingPassengers",
                newName: "PassengerId");

            migrationBuilder.RenameColumn(
                name: "DateOfBirth",
                table: "BookingPassengers",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "BookingPassengerId",
                table: "BookingPassengers",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Bookings",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Bookings",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "FlightId",
                table: "BookingPassengers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsContinuingPassenger",
                table: "BookingPassengers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PassengerEmail",
                table: "BookingPassengers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassengerName",
                table: "BookingPassengers",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SeatNumber",
                table: "BookingPassengers",
                type: "nvarchar(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_FlightPrice_Temporal_Bundle",
                table: "FlightPrices",
                columns: new[] { "FlightId", "CabinClass", "FlightBundleId", "EffectiveFrom" });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_BookingCode",
                table: "Bookings",
                column: "BookingCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPassengers_FlightId_SeatNumber",
                table: "BookingPassengers",
                columns: new[] { "FlightId", "SeatNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingPassengers_PassengerId",
                table: "BookingPassengers",
                column: "PassengerId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingPassengers_Passengers_PassengerId",
                table: "BookingPassengers",
                column: "PassengerId",
                principalTable: "Passengers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_UserId",
                table: "Bookings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
