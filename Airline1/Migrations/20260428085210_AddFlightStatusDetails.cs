using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airline1.Migrations
{
    /// <inheritdoc />
    public partial class AddFlightStatusDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActualArrivalTime",
                table: "Flights",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualDepartureTime",
                table: "Flights",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArrivalGate",
                table: "Flights",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArrivalTerminal",
                table: "Flights",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DelayMinutes",
                table: "Flights",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartureGate",
                table: "Flights",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartureTerminal",
                table: "Flights",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedArrivalTime",
                table: "Flights",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualArrivalTime",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "ActualDepartureTime",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "ArrivalGate",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "ArrivalTerminal",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "DelayMinutes",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "DepartureGate",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "DepartureTerminal",
                table: "Flights");

            migrationBuilder.DropColumn(
                name: "EstimatedArrivalTime",
                table: "Flights");
        }
    }
}
