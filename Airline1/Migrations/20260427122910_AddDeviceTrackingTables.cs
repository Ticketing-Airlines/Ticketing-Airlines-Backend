using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airline1.Migrations
{
    /// <inheritdoc />
    public partial class AddDeviceTrackingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrackingDevices",
                columns: table => new
                {
                    DeviceId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    MqttTopic = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeviceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsOnline = table.Column<bool>(type: "bit", nullable: false),
                    LastPingAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingDevices", x => x.DeviceId);
                });

            migrationBuilder.CreateTable(
                name: "DeviceLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceLocations_TrackingDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "TrackingDevices",
                        principalColumn: "DeviceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceLocation_DeviceId",
                table: "DeviceLocations",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceLocation_Timestamp",
                table: "DeviceLocations",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceLocations");

            migrationBuilder.DropTable(
                name: "TrackingDevices");
        }
    }
}
