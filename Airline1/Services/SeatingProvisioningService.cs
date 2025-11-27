using Airline1.Dtos.Requests;
using Airline1.IRepositories;
using Airline1.IService;

namespace Airline1.Services
{
    public class SeatingProvisioningService(
        IAircraftConfigurationService configService,
        ISeatService seatService,
        IAircraftRepository aircraftRepo) : ISeatingProvisioningService
    {
        // ---------------------------------------------------------------------
        // NEW METHOD: Handles the wipe and rebuild process for configuration changes.
        // ---------------------------------------------------------------------
        public async Task RegenerateSeatsForAircraftAsync(int aircraftId)
        {
            // 1. Load Aircraft (Ensures the aircraft exists)
            var aircraft = await aircraftRepo.GetByIdAsync(aircraftId)
                ?? throw new KeyNotFoundException($"Aircraft {aircraftId} not found.");

            // 2. Load Configuration (Ensures the new configuration exists before deletion)
            _ = await configService.GetByIdAsync(aircraft.ConfigurationID)
                ?? throw new KeyNotFoundException(
                    $"Configuration {aircraft.ConfigurationID} not found."
                );

            // 3. DELETE all existing seats for this aircraft
            // NOTE: This assumes ISeatService now has a method for bulk deletion.
            await seatService.DeleteAllSeatsForAircraftAsync(aircraftId);

            // 4. Re-create seats using the standard provisioning logic
            // Since this method already loaded the necessary data (aircraft and config), 
            // calling the main provisioning method directly is the most efficient approach.
            await ProvisionSeatsForAircraftAsync(aircraftId);
        }

        // ---------------------------------------------------------------------
        // EXISTING METHOD: Handles initial seat generation.
        // ---------------------------------------------------------------------
        public async Task ProvisionSeatsForAircraftAsync(int aircraftId)
        {
            // 1. Load aircraft
            var aircraft = await aircraftRepo.GetByIdAsync(aircraftId)
                ?? throw new KeyNotFoundException($"Aircraft {aircraftId} not found.");

            // 2. Load configuration
            var config = await configService.GetByIdAsync(aircraft.ConfigurationID)
                ?? throw new KeyNotFoundException(
                    $"Configuration {aircraft.ConfigurationID} not found."
                );

            // 3. Loop cabins
            foreach (var cabin in config.CabinDetails)
            {
                var columns = ParseLayout(cabin.SeatMapLayout);

                // 4. Loop rows + columns
                for (int row = cabin.StartRow; row <= cabin.EndRow; row++)
                {
                    foreach (var col in columns)
                    {
                        var seatReq = new CreateSeatRequest
                        {
                            AircraftId = aircraftId,
                            SeatNumber = $"{row}{col}",
                            SeatClass = cabin.CabinName,
                            IsExitRow = IsExitRow(row, aircraft.Model),
                            // IsAvailable defaults to true
                        };

                        await seatService.CreateAsync(seatReq);
                    }
                }
            }
        }

        private static List<string> ParseLayout(string layout)
{
            // example: "3-3" => ["A","B","C","D","E","F"]
            var result = new List<string>();

            var parts = layout.Split('-');
    
            // Safety check for parsing integers
            if (parts.Length < 2 || 
                !int.TryParse(parts[0], out int left) || 
                !int.TryParse(parts[1], out int right) || 
                left < 0 || right < 0)
            {
                throw new FormatException($"Invalid seat map layout format: {layout}. Expected format 'X-Y' where X and Y are non-negative integers.");
            }

            int totalSeats = left + right;

            for (int i = 0; i < totalSeats; i++)
                // Converts 0 to 'A', 1 to 'B', etc., and adds to the list
                result.Add(((char)('A' + i)).ToString());

            // The return type is now List<string>.
            return result;
}

        private static bool IsExitRow(int row, string model)
        {
            if (model.Contains("737", StringComparison.OrdinalIgnoreCase))
                return row == 15 || row == 16;

            return false;
        }
    }
}