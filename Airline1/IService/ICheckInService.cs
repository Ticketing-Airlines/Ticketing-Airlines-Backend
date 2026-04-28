using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;

namespace Airline1.IService
{
    public interface ICheckInService
    {
        Task<CheckInEligibilityResponse> GetEligibilityAsync(string bookingReference);
        Task<CheckInVerifyResponse> VerifyAsync(CheckInVerifyRequest request);
        Task<CheckInCompleteResponse> CompleteAsync(CheckInCompleteRequest request);
    }
}