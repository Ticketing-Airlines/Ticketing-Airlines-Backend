using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using AutoMapper;

namespace Airline1.Services
{
    public class FlightService(IFlightRepository repository, IMapper mapper, IWeatherService weatherService) : IFlightService
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

        public async Task<FlightStatusApiResponse> GetFlightStatusLookupAsync(string flightNumber, string date)
        {
            // Validate flight number format: 2 letters + space + 3-4 digits
            var flightNumberRegex = new System.Text.RegularExpressions.Regex(@"^[A-Z]{2}\s\d{3,4}$");
            if (!flightNumberRegex.IsMatch(flightNumber.ToUpperInvariant()))
            {
                return new FlightStatusApiResponse
                {
                    Success = false,
                    Error = "INVALID_FLIGHT_NUMBER",
                    Message = "Invalid flight number format. Example: SS 101"
                };
            }

            // Validate date format
            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var parsedDate))
            {
                return new FlightStatusApiResponse
                {
                    Success = false,
                    Error = "INVALID_DATE",
                    Message = "Invalid date format. Please use YYYY-MM-DD format."
                };
            }

            var flight = await repository.GetByFlightNumberAndDateAsync(flightNumber.ToUpperInvariant(), parsedDate);

            if (flight == null)
            {
                return new FlightStatusApiResponse
                {
                    Success = false,
                    Error = "FLIGHT_NOT_FOUND",
                    Message = "Flight not found. Please check the flight number and date."
                };
            }

            var status = DetermineFlightStatus(flight);
            var duration = flight.ArrivalTime - flight.DepartureTime;
            var durationFormatted = $"{duration.Hours}h {duration.Minutes}m";

            var departure = new AirportInfoResponse
            {
                Code = flight.Route?.OriginAirport?.IataCode ?? "",
                Airport = flight.Route?.OriginAirport?.Name ?? "",
                City = flight.Route?.OriginAirport?.City ?? "",
                Terminal = flight.DepartureTerminal,
                Gate = flight.DepartureGate,
                ScheduledTime = flight.DepartureTime.ToString("HH:mm"),
                ActualTime = flight.ActualDepartureTime?.ToString("HH:mm")
            };

            var arrival = new AirportInfoResponse
            {
                Code = flight.Route?.DestinationAirport?.IataCode ?? "",
                Airport = flight.Route?.DestinationAirport?.Name ?? "",
                City = flight.Route?.DestinationAirport?.City ?? "",
                Terminal = flight.ArrivalTerminal,
                Gate = flight.ArrivalGate,
                ScheduledTime = flight.ArrivalTime.ToString("HH:mm"),
                EstimatedTime = flight.EstimatedArrivalTime?.ToString("HH:mm")
            };

            // Weather integration
            var originAirport = flight.Route?.OriginAirport;
            var destAirport = flight.Route?.DestinationAirport;

            var departureWeather = new WeatherLocationResponse { Temp = "--°C", Condition = "N/A" };
            var arrivalWeather = new WeatherLocationResponse { Temp = "--°C", Condition = "N/A" };

            if (originAirport?.Latitude.HasValue == true && originAirport?.Longitude.HasValue == true)
            {
                try
                {
                    var weather = await weatherService.GetCurrentWeatherAsync(originAirport.Latitude.Value, originAirport.Longitude.Value);
                    departureWeather = new WeatherLocationResponse { Temp = weather.Temp, Condition = weather.Condition };
                }
                catch
                {
                    // fallback already handled in service
                }
            }

            if (destAirport?.Latitude.HasValue == true && destAirport?.Longitude.HasValue == true)
            {
                try
                {
                    var weather = await weatherService.GetCurrentWeatherAsync(destAirport.Latitude.Value, destAirport.Longitude.Value);
                    arrivalWeather = new WeatherLocationResponse { Temp = weather.Temp, Condition = weather.Condition };
                }
                catch
                {
                    // fallback already handled in service
                }
            }

            var response = new FlightStatusLookupResponse
            {
                FlightNumber = flight.FlightNumber,
                Airline = flight.Airline?.Name ?? "",
                Aircraft = flight.Aircraft == null ? "" : $"{flight.Aircraft.Manufacturer} {flight.Aircraft.Model}",
                Date = flight.DepartureTime.ToString("MMMM dd, yyyy"),
                Status = status,
                Duration = durationFormatted,
                Departure = departure,
                Arrival = arrival,
                Weather = new WeatherInfoResponse
                {
                    Departure = departureWeather,
                    Arrival = arrivalWeather
                }
            };

            return new FlightStatusApiResponse
            {
                Success = true,
                Data = response
            };
        }

        private static string DetermineFlightStatus(Flight flight)
        {
            var now = DateTime.Now;
            var scheduledDeparture = flight.DepartureTime;
            var scheduledArrival = flight.ArrivalTime;

            // Check cancelled first
            var latestStatus = flight.Statuses?.OrderByDescending(s => s.EffectiveAt).FirstOrDefault();
            if (latestStatus?.Status == Common.FlightStatusType.Cancelled)
            {
                return "Cancelled";
            }

            // If actual arrival is recorded and passed
            if (flight.ActualArrivalTime.HasValue && flight.ActualArrivalTime.Value <= now)
            {
                return "Arrived";
            }

            // If actual departure is recorded and passed, and estimated arrival not yet reached
            if (flight.ActualDepartureTime.HasValue && flight.ActualDepartureTime.Value <= now)
            {
                var estimatedArrival = flight.EstimatedArrivalTime ?? flight.ArrivalTime;
                if (now < estimatedArrival)
                {
                    return "Departed";
                }
                return "Arrived";
            }

            // Check delay
            if (flight.DelayMinutes.HasValue && flight.DelayMinutes.Value > 15)
            {
                return "Delayed";
            }

            // Check if within 15 minutes of scheduled departure
            var minutesUntilDeparture = (scheduledDeparture - now).TotalMinutes;
            if (minutesUntilDeparture <= 15 && minutesUntilDeparture >= -15)
            {
                return "On Time";
            }

            // If more than 2 hours away
            if (minutesUntilDeparture > 120)
            {
                return "Scheduled";
            }

            // Within 2 hours but not within 15 minutes, and no significant delay
            return "On Time";
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
