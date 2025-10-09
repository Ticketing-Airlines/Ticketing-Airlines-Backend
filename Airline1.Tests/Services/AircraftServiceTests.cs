using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Airline1.Services.Implementations;
using Airline1.Repositories.Interfaces;
using Airline1.Dtos.Requests;
using Airline1.Dtos.Responses;
using Airline1.Models;
using AutoMapper;
using System;

namespace Airline1.Tests.Services
{
    public class AircraftServiceTests
    {
        private readonly Mock<IAircraftRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AircraftService _service;

        public AircraftServiceTests()
        {
            _mockRepo = new Mock<IAircraftRepository>();
            _mockMapper = new Mock<IMapper>();

            _mockMapper.Setup(m => m.Map<Aircraft>(It.IsAny<CreateAircraftRequest>())).Returns((CreateAircraftRequest r) => new Aircraft {
                Model = r.Model,
                Manufacturer = r.Manufacturer,
                TailNumber = r.TailNumber,
                Registration = r.Registration
            });

            _mockMapper.Setup(m => m.Map<AircraftResponse>(It.IsAny<Aircraft>())).Returns((Aircraft a) => new AircraftResponse {
                Id = a.Id,
                Model = a.Model,
                Manufacturer = a.Manufacturer,
                TailNumber = a.TailNumber,
                Registration = a.Registration,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            });

            _mockMapper.Setup(m => m.Map(It.IsAny<UpdateAircraftRequest>(), It.IsAny<Aircraft>()))
                .Callback((UpdateAircraftRequest src, Aircraft dest) => {
                    if (src.Model != null) dest.Model = src.Model;
                    if (src.Manufacturer != null) dest.Manufacturer = src.Manufacturer;
                    if (src.TailNumber != null) dest.TailNumber = src.TailNumber;
                    if (src.Registration != null) dest.Registration = src.Registration;
                });

            _service = new AircraftService(_mockRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task CreateAsync_Succeeds_WhenTailUnique()
        {
            var req = new CreateAircraftRequest { Model = "A320", Manufacturer = "AirCorp", TailNumber = "TN1" };
            _mockRepo.Setup(r => r.GetByTailNumberAsync("TN1")).ReturnsAsync((Aircraft)null);
            var saved = new Aircraft { Id = 11, Model = "A320", Manufacturer = "AirCorp", TailNumber = "TN1", CreatedAt = DateTime.UtcNow };
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Aircraft>())).ReturnsAsync(saved);

            var res = await _service.CreateAsync(req);
            Assert.NotNull(res);
            Assert.Equal(11, res.Id);
            Assert.Equal("TN1", res.TailNumber);
        }

        [Fact]
        public async Task CreateAsync_Throws_WhenTailDuplicate()
        {
            var req = new CreateAircraftRequest { Model = "A320", Manufacturer = "AirCorp", TailNumber = "TN1" };
            var existing = new Aircraft { Id = 5, TailNumber = "TN1" };
            _mockRepo.Setup(r => r.GetByTailNumberAsync("TN1")).ReturnsAsync(existing);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateAsync(req));
        }

        [Fact]
        public async Task GetAllAsync_ReturnsMapped()
        {
            var items = new List<Aircraft> {
                new Aircraft { Id = 1, Model = "A320", Manufacturer = "AirCorp" },
                new Aircraft { Id = 2, Model = "B737", Manufacturer = "FlyInc" }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(items);
            var res = await _service.GetAllAsync();
            Assert.Equal(2, res.Count);
            Assert.Equal("A320", res[0].Model);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Aircraft)null);
            var res = await _service.GetByIdAsync(99);
            Assert.Null(res);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsMapped_WhenFound()
        {
            var item = new Aircraft { Id = 2, Model = "B737", Manufacturer = "FlyInc", CreatedAt = DateTime.UtcNow };
            _mockRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(item);
            var res = await _service.GetByIdAsync(2);
            Assert.NotNull(res);
            Assert.Equal(2, res.Id);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNull_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Aircraft)null);
            var req = new UpdateAircraftRequest { Model = "X" };
            var res = await _service.UpdateAsync(99, req);
            Assert.Null(res);
        }

        [Fact]
        public async Task UpdateAsync_Succeeds()
        {
            var existing = new Aircraft { Id = 5, Model = "Old", Manufacturer = "OldCo" };
            _mockRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Aircraft>())).Returns(Task.CompletedTask);
            var req = new UpdateAircraftRequest { Model = "NewModel" };
            var res = await _service.UpdateAsync(5, req);
            Assert.NotNull(res);
            Assert.Equal("NewModel", res.Model);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenExists()
        {
            var existing = new Aircraft { Id = 7, Model = "M" };
            _mockRepo.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.DeleteAsync(existing)).Returns(Task.CompletedTask);
            var res = await _service.DeleteAsync(7);
            Assert.True(res);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Aircraft)null);
            var res = await _service.DeleteAsync(99);
            Assert.False(res);
        }
    }
}
