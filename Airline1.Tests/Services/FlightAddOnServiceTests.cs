using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IRepositories;
using Airline1.Models;
using System.Collections.Generic;

namespace Airline1.Tests.Services
{
    public class FlightAddOnServiceTests
    {
        private readonly Mock<IFlightAddOnRepository> _mockRepo;
        private readonly FlightAddOnService _service;

        public FlightAddOnServiceTests()
        {
            _mockRepo = new Mock<IFlightAddOnRepository>();
            _service = new FlightAddOnService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            var list = new List<Airline1.Models.FlightAddOn> { new Airline1.Models.FlightAddOn { 
                Id = 1, 
                Name = "mockname",
                Code = "AIAA"} };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(list);
            var res = await _service.GetAllAsync();
            Assert.Single(res);
        }
    }
}
