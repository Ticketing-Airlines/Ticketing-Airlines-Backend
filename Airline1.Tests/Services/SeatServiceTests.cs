using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IRepositories;
using Airline1.Dtos.Requests;
using Airline1.Models;
using System.Collections.Generic;
using AutoMapper;

namespace Airline1.Tests.Services
{
    public class SeatServiceTests
    {
        private readonly Mock<ISeatRepository> _mockRepo;
        private readonly SeatService _service;
        private readonly Mock<IMapper> _mapper;

        public SeatServiceTests()
        {
            _mockRepo = new Mock<ISeatRepository>();
            _service = new SeatService(_mockRepo.Object, _mapper.Object);
        }

        [Fact]
        public async Task GetByAircraftAsync_ReturnsList()
        {
            var list = new List<Seat> { new Seat { 
                AircraftId =  1,
                SeatNumber =  "A1",
                
                Id = 1 } };
            _mockRepo.Setup(r => r.GetByAircraftAsync(1)).ReturnsAsync(list);
            var res = await _service.GetByAircraftAsync(1);
            Assert.Single(res);
        }
    }
}
