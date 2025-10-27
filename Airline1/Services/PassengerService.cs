using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.Models;
using Airline1.IService;

namespace Airline1.Services
{
    public class PassengerService(IPassengerRepository repo) : IPassengerService
    {
        public async Task<IEnumerable<PassengerResponse>> GetAllAsync()
        {
            var passengers = await repo.GetAllAsync();
            return passengers.Select(MapToResponse);
        }

        public async Task<PassengerResponse?> GetByIdAsync(int id)
        {
            var passenger = await repo.GetByIdAsync(id);
            return passenger == null ? null : MapToResponse(passenger);
        }

        public async Task<PassengerResponse> CreateAsync(CreatePassengerRequest request)
        {
            var passenger = new Passenger
            {
                UserId = request.UserId,
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName,
                Suffix = request.Suffix,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Nationality = request.Nationality,
                PassportNumber = request.PassportNumber,
                PassportExpiry = request.PassportExpiry,
                SpecialAssistance = request.SpecialAssistance
            };

            var created = await repo.AddAsync(passenger);
            return MapToResponse(created);
        }

        public async Task<PassengerResponse?> UpdateAsync(int id, CreatePassengerRequest request)
        {
            var existing = await repo.GetByIdAsync(id);
            if (existing == null) return null;

            existing.FirstName = request.FirstName;
            existing.MiddleName = request.MiddleName;
            existing.LastName = request.LastName;
            existing.Suffix = request.Suffix;
            existing.DateOfBirth = request.DateOfBirth;
            existing.Gender = request.Gender;
            existing.Email = request.Email;
            existing.PhoneNumber = request.PhoneNumber;
            existing.Nationality = request.Nationality;
            existing.PassportNumber = request.PassportNumber;
            existing.PassportExpiry = request.PassportExpiry;
            existing.SpecialAssistance = request.SpecialAssistance;
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await repo.UpdateAsync(existing);
            return MapToResponse(updated!);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await repo.DeleteAsync(id);
        }

        private static PassengerResponse MapToResponse(Passenger p) => new()
        {
            Id = p.Id,
            UserId = p.UserId,
            FlightId = p.FlightId,
            FullName = p.FullName,
            Gender = p.Gender,
            Email = p.Email,
            PhoneNumber = p.PhoneNumber,
            Nationality = p.Nationality,
            PassportNumber = p.PassportNumber,
            PassportExpiry = p.PassportExpiry,
            SpecialAssistance = p.SpecialAssistance,
            IsActive = p.IsActive
        };
    }
}
