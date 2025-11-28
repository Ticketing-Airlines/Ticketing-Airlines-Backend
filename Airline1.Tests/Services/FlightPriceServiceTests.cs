using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IRepositories;
using Airline1.Models;
using Airline1.Dtos.Requests;
using System.Collections.Generic;
using AutoMapper;

namespace Airline1.Tests.Services
{
    public class FlightPriceServiceTests
    {
        private readonly Mock<IFlightPriceRepository> _mockRepo;
        private readonly FlightPriceService _service;
        private readonly Mock<IMapper> _mapper;

        public FlightPriceServiceTests()
        {
            _mockRepo = new Mock<IFlightPriceRepository>();
            _service = new FlightPriceService(_mockRepo.Object,_mapper.Object);
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreated()
        {
            var req = new CreateFlightPriceRequest()
            {
                FlightBundleId = 1,
                BasePrice = 100.0m,
                FlightId = 1,
                PassengerType = "Adult"
            };
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<FlightPrice>()));
            var res = await _service.CreateAsync(req);
            Assert.Equal(1, res.Id);
        }
    }
}
