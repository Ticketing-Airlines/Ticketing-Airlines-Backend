//using Airline1.IRepositories;
//using Airline1.Models;
//using Airline1.Services;
//using Moq;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using Xunit;

//namespace Airline1.Tests.Services
//{
//    public class FlightSeatServiceTests
//    {
//        private readonly Mock<IFlightSeatRepository> _mockRepo;
//        private readonly FlightSeatService _service;

//        public FlightSeatServiceTests()
//        {
//            _mockRepo = new Mock<IFlightSeatRepository>();
//            _service = new FlightSeatService(_mockRepo.Object);
//        }

//        [Fact]
//        public async Task GetByFlightAsync_ReturnsList()
//        {
//            var list = new List<FlightSeat> { 
//                FlightId = 1,
//                int SeatId
//                string SeatClass string Status
//                Id = 1 } };
//            _mockRepo.Setup(r => r.GetByFlightAsync(1)).ReturnsAsync(list);
//            var res = await _service.GetByFlightAsync(1);
//            Assert.Single(res);
//        }
//    }
//}
