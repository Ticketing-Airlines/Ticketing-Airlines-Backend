using Xunit;
using Moq;
using Airline1.Controllers;
using Airline1.IService;
using Airline1.Dtos.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Airline1.Tests.Controllers
{
    //public class BookingsControllerTests
    //{
    //    private readonly Mock<IBookingService> _mockService;
    //    private readonly BookingsController _controller;

    //    public BookingsControllerTests()
    //    {
    //        _mockService = new Mock<IBookingService>();
    //        _controller = new BookingsController(_mockService.Object);
    //    }

    //    [Fact]
    //    public async Task Create_ReturnsCreated_WhenSuccessful()
    //    {
    //        var req = new CreateBookingRequest();
    //        var booking = new { Id = 1 };
    //        _mockService.Setup(s => s.CreateBookingAsync(req)).ReturnsAsync(booking);
    //        var result = await _controller.Create(req);
    //        Assert.IsType<CreatedAtActionResult>(result);
    //    }

    //    [Fact]
    //    public async Task GetById_ReturnsNotFound_WhenMissing()
    //    {
    //        _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((object?)null);
    //        var result = await _controller.GetById(99);
    //        Assert.IsType<NotFoundResult>(result);
    //    }

    //    [Fact]
    //    public async Task GetByFlight_ReturnsOk_List()
    //    {
    //        var list = new List<object> { new { Id = 1 } };
    //        _mockService.Setup(s => s.GetByFlightAsync(1)).ReturnsAsync(list);
    //        var result = await _controller.GetByFlight(1);
    //        var ok = Assert.IsType<OkObjectResult>(result);
    //        Assert.Equal(list, ok.Value);
    //    }
    //}
}
