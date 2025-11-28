// using Xunit;
// using Moq;
// using Airline1.Services;
// using Airline1.IRepositories;
// using Airline1.Models;
// using System.Collections.Generic;

// using AutoMapper;

// namespace Airline1.Tests.Services
// {
//     public class AddOnPriceServiceTests
//     {
//         private readonly Mock<IAddOnPriceRepository> _mockPriceRepo;
//         private readonly Mock< IFlightAddOnRepository> _mockFlightRepo;
//         private readonly AddOnPriceService _service;
//         private readonly Mock<IMapper> _mapper;

//         public AddOnPriceServiceTests()
//         {
//             _mockPriceRepo = new Mock<IAddOnPriceRepository>();
//             _mockFlightRepo = new Mock<IFlightAddOnRepository>();
//             _mapper = new Mock<IMapper>();
//             _service = new AddOnPriceService(_mockPriceRepo.Object,_mockFlightRepo.Object,_mapper.Object);
//         }

//         [Fact]
//         public async Task GetAllAsync_ReturnsList()
//         {
//             var list = new List<AddOnPrice> { new AddOnPrice { Id = 1 } };
//             _mockPriceRepo.Setup(r => r.GetAllByFlightAddOnAsync(1)).ReturnsAsync(list);
//             var res = await _service.GetByIdAsync(1);
//             Assert.Single(res); 
//         }
//     }
// }
