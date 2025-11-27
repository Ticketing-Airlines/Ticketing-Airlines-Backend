using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airline1.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFlightPriceToUseBundles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. RENAME COLUMN
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "FlightPrices",
                newName: "FlightBundleId");

            // ⭐ CRITICAL FIX: Replace the DropIndex call with conditional raw SQL.
            string indexToDrop = "IX_FlightPrices_FlightId_CabinClass_Type_EffectiveFrom";
            migrationBuilder.Sql($@"
        IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = '{indexToDrop}' AND object_id = OBJECT_ID('FlightPrices'))
        BEGIN
            DROP INDEX [{indexToDrop}] ON [FlightPrices];
        END;
    ");

            // 2. CREATE THE NEW COMPOSITE TEMPORAL INDEX
            migrationBuilder.CreateIndex(
                name: "IX_FlightPrice_Temporal_Bundle",
                table: "FlightPrices",
                columns: new[] { "FlightId", "CabinClass", "FlightBundleId", "EffectiveFrom" });

            // 3. CREATE THE FK INDEX
            migrationBuilder.CreateIndex(
                name: "IX_FlightPrices_FlightBundleId",
                table: "FlightPrices",
                column: "FlightBundleId");

            // 4. ADD THE FOREIGN KEY
            migrationBuilder.AddForeignKey(
                name: "FK_FlightPrices_FlightBundles_FlightBundleId",
                table: "FlightPrices",
                column: "FlightBundleId",
                principalTable: "FlightBundles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. DROP THE FOREIGN KEY
            migrationBuilder.DropForeignKey(
                name: "FK_FlightPrices_FlightBundles_FlightBundleId",
                table: "FlightPrices");

            // 2. DROP THE NEW INDICES
            migrationBuilder.DropIndex(
                name: "IX_FlightPrices_FlightBundleId",
                table: "FlightPrices");

            migrationBuilder.DropIndex(
                name: "IX_FlightPrice_Temporal_Bundle",
                table: "FlightPrices");

            // 3. RENAME COLUMN (Reverts FlightBundleId back to Type)
            migrationBuilder.RenameColumn(
                name: "FlightBundleId",
                table: "FlightPrices",
                newName: "Type");

            // 4. RECREATE THE OLD COMPOSITE INDEX
            migrationBuilder.CreateIndex(
                name: "IX_FlightPrices_FlightId_CabinClass_Type_EffectiveFrom",
                table: "FlightPrices",
                columns: new[] { "FlightId", "CabinClass", "Type", "EffectiveFrom" });
        }
    }
}