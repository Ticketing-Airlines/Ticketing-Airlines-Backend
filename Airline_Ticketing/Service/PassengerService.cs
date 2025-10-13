using Airline_Ticketing.Data;
using Airline_Ticketing.DTOs.Request;
using Airline_Ticketing.DTOs.Response;
using Airline_Ticketing.IServices;
using Airline_Ticketing.Model; 
using Microsoft.EntityFrameworkCore;


namespace Airline_Ticketing.Service


{
    public class PassengerService : IPassengerService
    {

        private readonly ApplicationDbContext _context;

        // We inject the database context here
        public PassengerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PassengerResponse> CreatePassengerAsync(CreatePassengerRequest request)
        {
            //Convert the "Request DTO" into a "Database Model"
            var newPassenger = new Passengers
            {
                Name = request.Name, 
                DateOfBirth = request.DateOfBirth,
                PassportNumber = request.PassportNumber,
                Nationality = request.Nationality
            };

            // Add it to the database and save
            _context.Passengers.Add(newPassenger);
            await _context.SaveChangesAsync();

            // 3. Convert the new database model back into a "Response DTO" to return it
            return new PassengerResponse
            {
                Id = newPassenger.PassengerID, 
                Name = newPassenger.Name, 
                DateOfBirth = newPassenger.DateOfBirth,
                Nationality = newPassenger.Nationality
            };
        }

        public async Task<IEnumerable<PassengerResponse>> GetAllPassengersAsync()
        {
            var passengers = await _context.Passengers
                .Select(p => new PassengerResponse 
                {
                    Id = p.PassengerID, 
                    Name = p.Name,       
                    DateOfBirth = p.DateOfBirth,
                    Nationality = p.Nationality
                })
                .ToListAsync();

            return passengers;
        }

        public async Task<PassengerResponse?> GetPassengerByIdAsync(int id)
        {
            var passenger = await _context.Passengers
                // First, find the passenger in the database using the correct ID field
                .FirstOrDefaultAsync(p => p.PassengerID == id); 

            if (passenger == null)
            {
                return null; // Not found
            }

            // If found, convert it to the response DTO
            return new PassengerResponse
            {
                Id = passenger.PassengerID, 
                Name = passenger.Name,      
                DateOfBirth = passenger.DateOfBirth,
                Nationality = passenger.Nationality
            };
        }

        public async Task<PassengerResponse?> UpdatePassengerAsync(int id, UpdatePassengerRequest request)
        {
            // Find the existing passenger in the database
            var passenger = await _context.Passengers
                .FirstOrDefaultAsync(p => p.PassengerID == id);

            if (passenger == null)
            {
                return null; // Passenger not found
            }

            // Update the passenger properties with new values
            passenger.Name = request.Name;
            passenger.DateOfBirth = request.DateOfBirth;
            passenger.PassportNumber = request.PassportNumber;
            passenger.Nationality = request.Nationality;

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Return the updated passenger as a response DTO
            return new PassengerResponse
            {
                Id = passenger.PassengerID,
                Name = passenger.Name,
                DateOfBirth = passenger.DateOfBirth,
                Nationality = passenger.Nationality
            };


        }

        public async Task<bool> DeletePassengerAsync(int id)
        {
            // Find the passenger to delete
            var passenger = await _context.Passengers
                .FirstOrDefaultAsync(p => p.PassengerID == id);

            if (passenger == null)
            {
                return false; // Passenger not found
            }

            // Remove the passenger from the database
            _context.Passengers.Remove(passenger);
            await _context.SaveChangesAsync();

            return true; // Successfully deleted
        }
    }
}

