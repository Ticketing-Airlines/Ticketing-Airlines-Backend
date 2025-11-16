using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Airline1.Services.Implementations;
using Airline1.Repositories.Interfaces;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.Models;
using AutoMapper;
using System;

namespace Airline1.Tests.Services
{
    public class AircraftServiceTests
    {
        private readonly Mock<IAircraftRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AircraftService _service;

        public AircraftServiceTests()
        {
            _mockRepo = new Mock<IAircraftRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockMapper.Setup(m => m.Map<Aircraft>(It.IsAny<CreateAircraftRequest>())).Returns((CreateAircraftRequest r) => new Aircraft {
                Model = r.Model,
                Manufacturer = r.Manufacturer,
                TailNumber = r.TailNumber,
                RegistrationNumber = r.RegistrationNumber,
                BaseAirport = new Airport {
                    Id = 1,
                    IataCode = "iata1",
                    IcaoCode = "icao1",
                    Name = "Airport1",
                    City = "City1",
                    Country = "Country1",
                    TimeZone = "TZ1" }
            });
            _service = new AircraftService(_mockRepo.Object, _mockMapper.Object);
        }
       [Fact]
        public async Task CreateAsync_ReturnsCreatedAircraft()
        {
            var request = new CreateAircraftRequest  
            {
                TailNumber = "01",
                Manufacturer = "AirCorp",
                Model = "TN1",
                RegistrationNumber = "1001", 
            };
            var response = new AircraftResponse
            {
                TailNumber = "01",
                Manufacturer = "AirCorp",
                Model = "TN1",
                RegistrationNumber = "1001",
                BaseAirportName = "Airport1"
            };
            var model = new Aircraft
            {
                TailNumber = "01",
                Manufacturer = "AirCorp",
                Model = "TN1",
                RegistrationNumber = "1001",
                CreatedAt = DateTime.UtcNow,
                BaseAirport = new Airport
                {
                    Id = 1,
                    IataCode = "iata1",
                    IcaoCode = "icao1",
                    Name = "Airport1",
                    City = "City1",
                    Country = "Country1",
                    TimeZone = "TZ1"
                }
            };
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Aircraft>())).ReturnsAsync(model);

            var result = await _service.CreateAsync(request);
            Assert.Equal(response, result);
        }


        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Aircraft)null);
            var res = await _service.DeleteAsync(99);
            Assert.False(res);
        }
    }
}
