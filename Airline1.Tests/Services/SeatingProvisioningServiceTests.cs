using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IService;
using Airline1.IRepositories;
using Airline1.Models;
using Airline1.Data;
using Microsoft.EntityFrameworkCore;

namespace Airline1.Tests.Services
{
    //public class SeatingProvisioningServiceTests
    //{
    //    private readonly SeatingProvisioningService _service;
    //    private readonly AppDbContext _db;
    //    private readonly IAircraftConfigurationService configService;
    //    private readonly ISeatService seatService;
    //    private readonly IAircraftRepository aircraftRepo;

    //    public SeatingProvisioningServiceTests()
    //    {
    //        configService = new IAircraftConfigurationService;
    //        var opt = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase("seatprov").Options;
    //        _db = new AppDbContext(opt);
    //        _service = new SeatingProvisioningService(configService, seatService, aircraftRepo);
    //    }

    //    [Fact]
    //    public async Task ProvisionSeats_CompletesWithoutThrow()
    //    {
    //        await _service.ProvisionSeatsForAircraftAsync(1);
    //        Assert.True(true);
    //    }
    //}
}
