using Xunit;
using Moq;
using Airline1.Services;
using Airline1.IRepositories;
using Airline1.Models;
using System.Collections.Generic;

namespace Airline1.Tests.Services
{
    public class AddOnPriceServiceTests
    {
        private readonly Mock<IAddOnPriceRepository> _mockRepo;
        private readonly AddOnPriceService _service;

        public AddOnPriceServiceTests()
        {
            _mockRepo = new Mock<IAddOnPriceRepository>();
            _service = new AddOnPriceService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsList()
        {
            var list = new List<AddOnPrice> { new AddOnPrice { Id = 1 } };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(list);
            var res = await _service.GetAllAsync();
            Assert.Single(res);
        }
    }
}
