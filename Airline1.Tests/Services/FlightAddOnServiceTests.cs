using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IRepositories;
using Airline1.Models;
using System.Collections.Generic;
using AutoMapper;

namespace Airline1.Tests.Services
{
    public class FlightAddOnServiceTests
    {
        private readonly Mock<IFlightAddOnRepository> _mockRepo;
        private readonly FlightAddOnService _service;
        private readonly Mock<IMapper> _mapper;

        public FlightAddOnServiceTests()
        {
            _mockRepo = new Mock<IFlightAddOnRepository>();
            _mapper = new Mock<IMapper>();
            _service = new FlightAddOnService(_mockRepo.Object, _mapper.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            var list = new List<Airline1.Models.FlightAddOn> { new Airline1.Models.FlightAddOn {
                Id = 1,
                Name = "mockname",
                Code = "AIAA"} };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(list);
            _mapper.Setup(m => m.Map<IEnumerable<Airline1.Dtos.Responses.FlightAddOnResponse>>(It.IsAny<IEnumerable<Airline1.Models.FlightAddOn>>()))
                .Returns(new List<Airline1.Dtos.Responses.FlightAddOnResponse> { new Airline1.Dtos.Responses.FlightAddOnResponse { Id = 1, Name = "mockname", Code = "AIAA" } });
            var res = await _service.GetAllAsync();
            Assert.Single(res);
        }
    }
}
