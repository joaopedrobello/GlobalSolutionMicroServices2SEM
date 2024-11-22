using Moq;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using GlobalSolution.Controllers;
using GlobalSolution.Models;
using GlobalSolution.Services;

namespace GlobalSolution.Tests
{
    public class ConsumoControllerTests
    {
        private readonly Mock<MongoDbService> _mockMongoDbService;
        private readonly Mock<CacheService> _mockCacheService;
        private readonly ConsumoController _controller;

        public ConsumoControllerTests()
        {
            _mockMongoDbService = new Mock<MongoDbService>();
            _mockCacheService = new Mock<CacheService>();
            _controller = new ConsumoController(_mockMongoDbService.Object, _mockCacheService.Object);
        }

        [Fact]
        public async Task ObterConsumos_ShouldReturnOk_WhenDataExists()
        {
            var consumos = new List<ConsumoModel>
            {
                new ConsumoModel { Device = "Device1", PowerUsage = 2.0, Timestamp = System.DateTime.Now }
            };
            _mockCacheService.Setup(s => s.GetCacheAsync<List<ConsumoModel>>("consumos")).ReturnsAsync(consumos);

            var result = await _controller.ObterConsumos();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task ObterConsumos_ShouldReturnNotFound_WhenNoData()
        {
            _mockCacheService.Setup(s => s.GetCacheAsync<List<ConsumoModel>>("consumos")).ReturnsAsync((List<ConsumoModel>)null);

            var result = await _controller.ObterConsumos();

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task RegistrarConsumo_ShouldReturnBadRequest_WhenInvalidData()
        {
            ConsumoModel invalidConsumo = null;

            var result = await _controller.RegistrarConsumo(invalidConsumo);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
    }
}
