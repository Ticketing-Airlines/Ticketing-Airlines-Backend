using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using AutoMapper;

namespace Airline1.Services
{
    public class FlightService(IFlightRepository repository, IMapper mapper) : IFlightService
    {
        public async Task<IEnumerable<FlightResponse>> SearchFlightsAsync(string origin, string destination, DateTime departureDate, int passengerCount)
        {
            var flights = await repository.SearchAsync(origin, destination, departureDate);
            return mapper.Map<IEnumerable<FlightResponse>>(flights);
        }

        public async Task<FlightSearchResponse> SearchFlightsAsync(SearchFlightRequest request)
        {
            // 1. Search outbound flights
            var outboundFlights = await repository.SearchAsync(request);

            // 2. Filter by capacity (available seats >= passengers)
            var filteredOutbound = outboundFlights
                .Where(f => (f.Seats?.Count(s => s.Status == "Available") ?? 0) >= request.Passengers)
                .ToList();

            // 3. Map to search result DTOs
            var outboundResults = filteredOutbound
                .Select(f => MapToSearchResult(f, request.Passengers))
                .ToList();

            // 4. Handle round-trip
            if (string.Equals(request.TripType, "round-trip", StringComparison.OrdinalIgnoreCase) && request.ReturnDate.HasValue)
            {
                var returnRequest = new SearchFlightRequest
                {
                    From = request.To,
                    To = request.From,
                    DepartureDate = request.ReturnDate.Value,
                    Passengers = request.Passengers,
                    TripType = request.TripType
                };

                var returnFlights = await repository.SearchAsync(returnRequest);
                var filteredReturn = returnFlights
                    .Where(f => (f.Seats?.Count(s => s.Status == "Available") ?? 0) >= request.Passengers)
                    .ToList();

                var returnResults = filteredReturn
                    .Select(f => MapToSearchResult(f, request.Passengers))
                    .ToList();

                // 5. Create valid combinations
                var combinations = new List<RoundTripCombinationResponse>();
                foreach (var outbound in outboundResults)
                {
                    foreach (var ret in returnResults)
                    {
                        combinations.Add(new RoundTripCombinationResponse
                        {
                            Outbound = outbound,
                            Return = ret,
                            TotalPrice = outbound.Price + ret.Price
                        });
                    }
                }

                // 6. Sort by total price (lowest first)
                combinations = combinations.OrderBy(c => c.TotalPrice).ToList();

                return new FlightSearchResponse
                {
                    Success = true,
                    Data = new FlightSearchData
                    {
                        Results = combinations.Cast<object>().ToList(),
                        SearchParams = request
                    }
                };
            }

            // 7. Sort one-way by price (lowest first)
            outboundResults = outboundResults.OrderBy(f => f.Price).ToList();

            return new FlightSearchResponse
            {
                Success = true,
                Data = new FlightSearchData
                {
                    Results = outboundResults.Cast<object>().ToList(),
                    SearchParams = request
                }
            };
        }

        private static FlightSearchResultResponse MapToSearchResult(Flight flight, int passengers)
        {
            var availableSeats = flight.Seats?.Count(s => s.Status == "Available") ?? 0;

            // Get lowest active price
            var activePrices = flight.FlightPrices?
                .Where(p => p.IsActive)
                .ToList() ?? new List<FlightPrice>();

            var lowestPrice = activePrices.Any()
                ? activePrices.Min(p => p.BasePrice)
                : 0m;

            var lowestPriceRecord = activePrices
                .OrderBy(p => p.BasePrice)
                .FirstOrDefault();

            var duration = flight.ArrivalTime - flight.DepartureTime;
            var durationFormatted = $"{duration.Hours}h {duration.Minutes}m";

            return new FlightSearchResultResponse
            {
                FlightInstanceId = flight.Id,
                FlightNumber = flight.FlightNumber,
                OriginAirport = flight.Route?.OriginAirport == null ? null : new AirportSearchResponse
                {
                    AirportId = flight.Route.OriginAirport.Id,
                    Name = flight.Route.OriginAirport.Name,
                    City = flight.Route.OriginAirport.City,
                    CountryIso2 = flight.Route.OriginAirport.CountryIso2 ?? flight.Route.OriginAirport.Country,
                    IataCode = flight.Route.OriginAirport.IataCode
                },
                DestinationAirport = flight.Route?.DestinationAirport == null ? null : new AirportSearchResponse
                {
                    AirportId = flight.Route.DestinationAirport.Id,
                    Name = flight.Route.DestinationAirport.Name,
                    City = flight.Route.DestinationAirport.City,
                    CountryIso2 = flight.Route.DestinationAirport.CountryIso2 ?? flight.Route.DestinationAirport.Country,
                    IataCode = flight.Route.DestinationAirport.IataCode
                },
                DepartureTime = flight.DepartureTime.ToString("HH:mm"),
                ArrivalTime = flight.ArrivalTime.ToString("HH:mm"),
                Duration = durationFormatted,
                Price = lowestPrice,
                Currency = "PHP",
                FareCode = lowestPriceRecord?.FlightBundle?.Code,
                AvailableSeats = availableSeats,
                Aircraft = flight.Aircraft == null ? null : new AircraftSearchResponse
                {
                    AircraftId = flight.Aircraft.Id,
                    Model = flight.Aircraft.Model,
                    Manufacturer = flight.Aircraft.Manufacturer,
                    Capacity = flight.Aircraft.Configuration?.TotalSeats ?? 0
                },
                Airline = flight.Airline == null ? null : new AirlineSearchResponse
                {
                    AirlineId = flight.Airline.Id,
                    Name = flight.Airline.Name,
                    IataCode = flight.Airline.IataCode,
                    Logo = flight.Airline.Logo
                }
            };
        }

        public async Task<FlightResponse?> GetFlightStatusAsync(string flightNumber, DateTime date)
        {
            var flight = await repository.GetByFlightNumberAndDateAsync(flightNumber, date);

            if (flight == null)
                return null;

            return mapper.Map<FlightResponse>(flight);
        }

        public async Task<IEnumerable<FlightResponse>> GetAllAsync()
        {
            var flights = await repository.GetAllAsync();
            return mapper.Map<IEnumerable<FlightResponse>>(flights);
        }

        public async Task<FlightResponse?> GetByIdAsync(int id)
        {
            var flight = await repository.GetByIdAsync(id);
            return flight == null ? null : mapper.Map<FlightResponse>(flight);
        }

        public async Task<FlightResponse> CreateAsync(CreateFlightRequest request)
        {
            var flight = mapper.Map<Flight>(request);
            await repository.AddAsync(flight);
            await repository.SaveChangesAsync();
            return mapper.Map<FlightResponse>(flight);
        }

        public async Task<FlightResponse?> UpdateAsync(int id, UpdateFlightRequest request)
        {
            var flight = await repository.GetByIdAsync(id);
            if (flight == null) return null;

            mapper.Map(request, flight);
            repository.UpdateAsync(flight);
            await repository.SaveChangesAsync();
            return mapper.Map<FlightResponse>(flight);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var flight = await repository.GetByIdAsync(id);
            if (flight == null) return false;

            repository.DeleteAsync(flight);
            await repository.SaveChangesAsync();
            return true;
        }
    }
}
