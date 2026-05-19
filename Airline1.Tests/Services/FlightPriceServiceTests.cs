using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IRepositories;
using Airline1.Models;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
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
            _mapper = new Mock<IMapper>();
            _service = new FlightPriceService(_mockRepo.Object, _mapper.Object);
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
            var entity = new FlightPrice { Id = 1, FlightId = 1, FlightBundleId = 1, BasePrice = 100.0m, PassengerType = "Adult", CabinClass = "Economy" };
            var response = new FlightPriceResponse { Id = 1, FlightId = 1, BasePrice = 100.0m, PassengerType = "Adult" };
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<FlightPrice>())).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mapper.Setup(m => m.Map<FlightPrice>(It.IsAny<CreateFlightPriceRequest>())).Returns(entity);
            _mapper.Setup(m => m.Map<FlightPriceResponse>(It.IsAny<FlightPrice>())).Returns(response);
            var res = await _service.CreateAsync(req);
            Assert.Equal(1, res.Id);
        }
    }
}
