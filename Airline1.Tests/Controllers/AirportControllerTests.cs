using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.Services.Interfaces;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airline1.Tests.Controllers
{
    public class AirportsControllerTests
    {
        private readonly Mock<IAirportService> _mockService;
        private readonly AirportsController _controller;

        public AirportsControllerTests()
        {
            _mockService = new Mock<IAirportService>();
            _controller = new AirportsController(_mockService.Object);
        }

        [Fact]
        public async Task Create_ReturnsCreatedAtAction_WhenValid()
        {
            var request = new CreateAirportRequest()
            {   
                IataCode = "iatacode", 
                IcaoCode = "iacaocode",
                Name = "name",
                City = "city",
                Country = "country",
                TimeZone = "timezone"
            };
            var response = new AirportResponse 
            {   
                Id = 1, 
                IataCode = "iatacode", 
                IcaoCode = "iacaocode",
                Name = "name",
                City = "city",
                Country = "country",
                TimeZone = "timezone"
            };

            _mockService.Setup(x => x.CreateAsync(request)).ReturnsAsync(response);

            var result = await _controller.Create(request);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
            Assert.Equal(response, createdResult.Value);
        }
    }
}