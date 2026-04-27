using Airline1.Data;
using Airline1.Dtos.Responses;
using Airline1.IService;
using Microsoft.EntityFrameworkCore;

namespace Airline1.Services
{
    public class TrackingService(AppDbContext db) : ITrackingService
    {
        public async Task<DeviceLocationResponse?> GetLatestLocationAsync(string deviceId)
        {
            var latest = await db.DeviceLocations
                .Where(dl => dl.DeviceId == deviceId)
                .OrderByDescending(dl => dl.Timestamp)
                .FirstOrDefaultAsync();

            if (latest == null) return null;

            return new DeviceLocationResponse
            {
                DeviceId = latest.DeviceId,
                Latitude = latest.Latitude,
                Longitude = latest.Longitude,
                LastUpdated = latest.Timestamp
            };
        }
    }
}
