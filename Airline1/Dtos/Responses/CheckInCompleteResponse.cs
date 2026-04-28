using System;
using System.Collections.Generic;

namespace Airline1.Dtos.Responses
{
    public class CheckInCompleteResponse
    {
        public required string BookingReference { get; set; }
        public required string Status { get; set; }
        public DateTime CheckedInAt { get; set; }
        public List<BoardingPassResponse>? BoardingPasses { get; set; }
    }

    public class BoardingPassResponse
    {
        public Guid BoardingPassId { get; set; }
        public required string PassengerName { get; set; }
        public required string FlightNumber { get; set; }
        public required string FromAirport { get; set; }
        public required string ToAirport { get; set; }
        public DateTime DepartureDate { get; set; }
        public required string SeatNumber { get; set; }
        public required string Barcode { get; set; }
        public string? QRCodeImageBase64 { get; set; }
        public string? BoardingPassUrl { get; set; }
    }
}