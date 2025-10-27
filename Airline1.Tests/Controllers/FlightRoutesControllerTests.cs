// using Xunit;
// using Moq;
// using Airline1.Controllers;
// using Airline1.Services.Interfaces;
// using Airline1.Dtos.Requests;
// using Airline1.Dtos.Responses;
// using Microsoft.AspNetCore.Mvc;
// using System.Collections.Generic;
// using System.Threading.Tasks;

// namespace Airline1.Tests.Controllers
// {
//     public class FlightRoutesControllerTests
//     {
//         private readonly Mock<IFlightRouteService> _mockService;
//         private readonly FlightRoutesController _controller;

//         public FlightRoutesControllerTests()
//         {
//             _mockService = new Mock<IFlightRouteService>();
//             _controller = new FlightRoutesController(_mockService.Object);
//         }

//         [Fact]
//         public async Task Create_ReturnsCreatedAtAction_WhenValid()
//         {
//             var request = new CreateFlightRouteRequest {
//                 Code = "FR1",
//                 OriginAirportId = 1,
//                 DestinationAirportId = 2,
//                 DistanceKm = 1000,
//                 AverageFlightTimeMinutes = 120,
//                 FrequencyWeekly = 7,
//                 IsActive = true
//             };
//             var response = new FlightRouteResponse {
//                 Id = 1,
//                 Code = "FR1",
//                 OriginAirportId = 1,
//                 OriginAirportName = "Origin",
//                 DestinationAirportId = 2,
//                 DestinationAirportName = "Dest",
//                 DistanceKm = 1000,
//                 AverageFlightTimeMinutes = 120,
//                 FrequencyWeekly = 7,
//                 IsActive = true,
//                 CreatedAt = System.DateTime.UtcNow,
//                 UpdatedAt = null
//             };
//             _mockService.Setup(x => x.CreateAsync(request)).ReturnsAsync(response);

//             var result = await _controller.Create(request);
//             var createdResult = Assert.IsType<CreatedAtActionResult>(result);
//             Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
//             Assert.Equal(response, createdResult.Value);
//         }

//         [Fact]
//         public async Task GetAll_ReturnsOkResult_WithListOfFlightRoutes()
//         {
//             var routes = new List<FlightRouteResponse> {
//                 new FlightRouteResponse {
//                     Id = 1,
//                     Code = "FR1",
//                     OriginAirportId = 1,
//                     OriginAirportName = "Origin1",
//                     DestinationAirportId = 2,
//                     DestinationAirportName = "Dest1",
//                     DistanceKm = 1000,
//                     AverageFlightTimeMinutes = 120,
//                     FrequencyWeekly = 7,
//                     IsActive = true,
//                     CreatedAt = System.DateTime.UtcNow,
//                     UpdatedAt = null
//                 },
//                 new FlightRouteResponse {
//                     Id = 2,
//                     Code = "FR2",
//                     OriginAirportId = 3,
//                     OriginAirportName = "Origin2",
//                     DestinationAirportId = 4,
//                     DestinationAirportName = "Dest2",
//                     DistanceKm = 2000,
//                     AverageFlightTimeMinutes = 180,
//                     FrequencyWeekly = 3,
//                     IsActive = true,
//                     CreatedAt = System.DateTime.UtcNow,
//                     UpdatedAt = null
//                 }
//             };
//             _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(routes);
//             var result = await _controller.GetAll();
//             var okResult = Assert.IsType<OkObjectResult>(result.Result);
//             var returnValue = Assert.IsType<List<FlightRouteResponse>>(okResult.Value);
//             Assert.Equal(2, returnValue.Count);
//         }

//         [Fact]
//         public async Task GetById_ReturnsOkResult_WhenExists()
//         {
//             var route = new FlightRouteResponse {
//                 Id = 1,
//                 Code = "FR1",
//                 OriginAirportId = 1,
//                 OriginAirportName = "Origin",
//                 DestinationAirportId = 2,
//                 DestinationAirportName = "Dest",
//                 DistanceKm = 1000,
//                 AverageFlightTimeMinutes = 120,
//                 FrequencyWeekly = 7,
//                 IsActive = true,
//                 CreatedAt = System.DateTime.UtcNow,
//                 UpdatedAt = null
//             };
//             _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(route);
//             var result = await _controller.GetById(1);
//             var okResult = Assert.IsType<OkObjectResult>(result.Result);
//             var returnValue = Assert.IsType<FlightRouteResponse>(okResult.Value);
//             Assert.Equal(1, returnValue.Id);
//         }

//         [Fact]
//         public async Task GetById_ReturnsNotFound_WhenNotExists()
//         {
//             _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((FlightRouteResponse?)null);
//             var result = await _controller.GetById(99);
//             Assert.IsType<NotFoundResult>(result.Result);
//         }

//         [Fact]
//         public async Task Update_ReturnsOk_WhenUpdateIsSuccessful()
//         {
//             var request = new UpdateFlightRouteRequest {
//                 Code = "FR1",
//                 OriginAirportId = 1,
//                 DestinationAirportId = 2,
//                 DistanceKm = 1000,
//                 AverageFlightTimeMinutes = 120,
//                 FrequencyWeekly = 5,
//                 IsActive = true
//             };
//             var updated = new FlightRouteResponse {
//                 Id = 1,
//                 Code = "FR1",
//                 OriginAirportId = 1,
//                 OriginAirportName = "Origin",
//                 DestinationAirportId = 2,
//                 DestinationAirportName = "Dest",
//                 DistanceKm = 1000,
//                 AverageFlightTimeMinutes = 120,
//                 FrequencyWeekly = 5,
//                 IsActive = true,
//                 CreatedAt = System.DateTime.UtcNow,
//                 UpdatedAt = System.DateTime.UtcNow
//             };
//             _mockService.Setup(s => s.UpdateAsync(1, request)).ReturnsAsync(updated);
//             var result = await _controller.Update(1, request);
//             var okResult = Assert.IsType<OkObjectResult>(result);
//             var returnValue = Assert.IsType<FlightRouteResponse>(okResult.Value);
//             Assert.Equal(5, returnValue.FrequencyWeekly);
//         }

//         [Fact]
//         public async Task Update_ReturnsNotFound_WhenNotExists()
//         {
//             var request = new UpdateFlightRouteRequest {
//                 Code = "FR99",
//                 OriginAirportId = 99,
//                 DestinationAirportId = 100,
//                 DistanceKm = 500,
//                 AverageFlightTimeMinutes = 60,
//                 FrequencyWeekly = 1,
//                 IsActive = false
//             };
//             _mockService.Setup(s => s.UpdateAsync(99, request)).ReturnsAsync((FlightRouteResponse?)null);
//             var result = await _controller.Update(99, request);
//             Assert.IsType<NotFoundResult>(result);
//         }

//         [Fact]
//         public async Task Delete_ReturnsNoContent_WhenDeleteIsSuccessful()
//         {
//             _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);
//             var result = await _controller.Delete(1);
//             Assert.IsType<NoContentResult>(result);
//         }

//         [Fact]
//         public async Task Delete_ReturnsNotFound_WhenNotExists()
//         {
//             _mockService.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);
//             var result = await _controller.Delete(99);
//             Assert.IsType<NotFoundResult>(result);
//         }
//     }
// }
