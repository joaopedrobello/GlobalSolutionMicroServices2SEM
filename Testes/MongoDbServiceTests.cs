using Moq;
using MongoDB.Driver;
using Xunit;
using System.Threading.Tasks;
using System;
using GlobalSolution.Models;
using GlobalSolution.Services;

namespace GlobalSolution.Tests
{
    public class MongoDbServiceTests
    {
        private readonly Mock<IMongoCollection<ConsumoModel>> _mockCollection;
        private readonly MongoDbService _mongoDbService;

        public MongoDbServiceTests()
        {
            _mockCollection = new Mock<IMongoCollection<ConsumoModel>>();
            var mockDb = new Mock<IMongoDatabase>();
            mockDb.Setup(db => db.GetCollection<ConsumoModel>("Consumos", null)).Returns(_mockCollection.Object);
            _mongoDbService = new MongoDbService((IConfiguration)mockDb.Object);
        }

        [Fact]
        public async Task InserirConsumo_Success()
        {
            var consumo = new ConsumoModel { Device = "Device1", PowerUsage = 1.5, Timestamp = DateTime.Now };

            await _mongoDbService.InsertConsumoAsync(consumo);

            _mockCollection.Verify(c => c.InsertOneAsync(It.IsAny<ConsumoModel>(), null, default), Times.Once);
        }
    }
}
