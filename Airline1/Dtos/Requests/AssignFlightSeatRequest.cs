using System.ComponentModel.DataAnnotations;

namespace Airline1.Dtos.Requests
{
    public class AssignFlightSeatRequest
    {
        public required int PassengerId { get; set; }
    }
}
