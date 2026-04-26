using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Airline1.Migrations
{
    public partial class ConvertAllIdsToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // --- PHASE 1: PREPARE THE USERS TABLE ---

            // 1. Drop existing "Bridges" (Foreign Keys) so we can move the foundation
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Bookings_Users_UserId') ALTER TABLE Bookings DROP CONSTRAINT FK_Bookings_Users_UserId;");
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Passengers_Users_UserId') ALTER TABLE Passengers DROP CONSTRAINT FK_Passengers_Users_UserId;");
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Bookings_UserId') DROP INDEX IX_Bookings_UserId ON Bookings;");
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Passengers_UserId') DROP INDEX IX_Passengers_UserId ON Passengers;");

            // 2. Drop the Primary Key
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_Users') ALTER TABLE Users DROP CONSTRAINT PK_Users;");

            // 3. Rename the old column and create the new String ID
            migrationBuilder.Sql("EXEC sp_rename 'Users.Id', 'Id_Old', 'COLUMN';");
            migrationBuilder.Sql("ALTER TABLE Users ADD Id nvarchar(450) NOT NULL DEFAULT '';");

            // 4. (Optional) Copy data: every user's Email becomes their new Id
            migrationBuilder.Sql("UPDATE Users SET Id = Email WHERE Email IS NOT NULL;");

            // 5. Re-apply the Primary Key stamp
            migrationBuilder.Sql("ALTER TABLE Users ADD CONSTRAINT PK_Users PRIMARY KEY (Id);");


            // --- PHASE 2: UPDATE THE LINKED TABLES (Bookings & Passengers) ---

            // 6. Change the UserId columns to String type
            migrationBuilder.Sql("ALTER TABLE Bookings ALTER COLUMN UserId nvarchar(450) NULL;");
            migrationBuilder.Sql("ALTER TABLE Passengers ALTER COLUMN UserId nvarchar(450) NULL;");

            // 7. Rebuild the Bridges
            migrationBuilder.Sql("ALTER TABLE Bookings ADD CONSTRAINT FK_Bookings_Users_UserId FOREIGN KEY (UserId) REFERENCES Users (Id);");
            migrationBuilder.Sql("ALTER TABLE Passengers ADD CONSTRAINT FK_Passengers_Users_UserId FOREIGN KEY (UserId) REFERENCES Users (Id);");


            // --- PHASE 3: THE PASSENGERS TABLE IDENTITY ---

            // 8. Handle the Passenger's own ID (Int to String)
            migrationBuilder.Sql("IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_Passengers') ALTER TABLE Passengers DROP CONSTRAINT PK_Passengers;");
            migrationBuilder.Sql("EXEC sp_rename 'Passengers.Id', 'Id_Old', 'COLUMN';");
            migrationBuilder.Sql("ALTER TABLE Passengers ADD Id nvarchar(450) NOT NULL DEFAULT '';");

            // 9. Generate temporary unique IDs for existing passengers (if any)
            migrationBuilder.Sql("UPDATE Passengers SET Id = CAST(Id_Old AS nvarchar(450));");
            migrationBuilder.Sql("ALTER TABLE Passengers ADD CONSTRAINT PK_Passengers PRIMARY KEY (Id);");

            // --- PHASE 4: CLEANUP ---
            migrationBuilder.Sql("ALTER TABLE Users DROP COLUMN Id_Old;");
            migrationBuilder.Sql("ALTER TABLE Passengers DROP COLUMN Id_Old;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            throw new NotSupportedException("Down migration not supported for this data conversion.");
        }
    }
}
