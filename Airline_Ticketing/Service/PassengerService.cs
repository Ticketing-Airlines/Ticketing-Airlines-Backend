using Airline_Ticketing.DTOs.Request;
using Airline_Ticketing.DTOs.Response;
using Airline_Ticketing.IRepository;
using Airline_Ticketing.IServices;
using Airline_Ticketing.Model;
using Microsoft.EntityFrameworkCore;


namespace Airline_Ticketing.Service


{
    public class PassengerService : IPassengerService
    {

        private readonly IPassengerRepository _passengerRepository;

        // We inject the passenger repository here
        public PassengerService(IPassengerRepository passengerRepository)
        {
            _passengerRepository = passengerRepository;
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

            // Add it to the database and save using repository
            var createdPassenger = await _passengerRepository.AddAsync(newPassenger);

            // Convert the new database model back into a "Response DTO" to return it
            return new PassengerResponse
            {
                Id = createdPassenger.PassengerID,
                Name = createdPassenger.Name,
                DateOfBirth = createdPassenger.DateOfBirth,
                Nationality = createdPassenger.Nationality
            };
        }

        public async Task<IEnumerable<PassengerResponse>> GetAllPassengersAsync()
        {
            var passengers = await _passengerRepository.GetAllAsync();

            return passengers.Select(p => new PassengerResponse
            {
                Id = p.PassengerID,
                Name = p.Name,
                DateOfBirth = p.DateOfBirth,
                Nationality = p.Nationality
            });
        }

        public async Task<PassengerResponse?> GetPassengerByIdAsync(int id)
        {
            var passenger = await _passengerRepository.GetByIdAsync(id);

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
            var passenger = await _passengerRepository.GetByIdAsync(id);

            if (passenger == null)
            {
                return null; // Passenger not found
            }

            // Update the passenger properties with new values
            passenger.Name = request.Name;
            passenger.DateOfBirth = request.DateOfBirth;
            passenger.PassportNumber = request.PassportNumber;
            passenger.Nationality = request.Nationality;

            // Save the changes to the database using repository
            var updatedPassenger = await _passengerRepository.UpdateAsync(passenger);

            // Return the updated passenger as a response DTO
            return new PassengerResponse
            {
                Id = updatedPassenger.PassengerID,
                Name = updatedPassenger.Name,
                DateOfBirth = updatedPassenger.DateOfBirth,
                Nationality = updatedPassenger.Nationality
            };
        }

        public async Task<bool> DeletePassengerAsync(int id)
        {
            return await _passengerRepository.DeleteAsync(id);
        }
    }
}

