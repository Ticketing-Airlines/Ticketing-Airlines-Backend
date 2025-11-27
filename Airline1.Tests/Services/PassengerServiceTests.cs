using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IRepositories;
using Airline1.Models;
using Airline1.Dtos.Requests;
using System.Collections.Generic;

namespace Airline1.Tests.Services
{
    public class PassengerServiceTests
    {
        private readonly Mock<IPassengerRepository> _mockRepo;
        private readonly PassengerService _service;

        public PassengerServiceTests()
        {
            _mockRepo = new Mock<IPassengerRepository>();
            _service = new PassengerService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsMappedList()
        {
            var list = new List<Passenger> { new Passenger { Id = 1, FirstName = "A", LastName = "B" } };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(list);
            var res = await _service.GetAllAsync();
            Assert.Single(res);
            Assert.Equal(1, res.First().Id);
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreated()
        {
            var req = new CreatePassengerRequest { FirstName = "A", LastName = "B" };
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Passenger>())).ReturnsAsync(new Passenger { Id = 2, FirstName = "A", LastName = "B" });
            var res = await _service.CreateAsync(req);
            Assert.Equal(2, res.Id);
        }
    }
}
