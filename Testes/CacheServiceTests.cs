using Moq;
using StackExchange.Redis;
using Newtonsoft.Json;
using Xunit;
using System.Threading.Tasks;
using GlobalSolution.Services;

namespace GlobalSolution.Tests
{
    public class CacheServiceTests
    {
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly CacheService _cacheService;

        public CacheServiceTests()
        {
            _mockDatabase = new Mock<IDatabase>();
            _cacheService = new CacheService((IConfiguration)_mockDatabase.Object);
        }

        [Fact]
        public async Task SetCacheAsync_ShouldSetCache()
        {
            var key = "testKey";
            var value = new { Device = "Device1", PowerUsage = 1.5 };
            var serializedValue = JsonConvert.SerializeObject(value);

            await _cacheService.SetCacheAsync(key, value);

            _mockDatabase.Verify(db => db.StringSetAsync(key, serializedValue, It.IsAny<TimeSpan>(), When.Always, CommandFlags.None), Times.Once);
        }

        [Fact]
        public async Task GetCacheAsync_ShouldReturnCachedValue()
        {
            var key = "testKey";
            var cachedValue = "{\"Device\":\"Device1\",\"PowerUsage\":1.5}";
            _mockDatabase.Setup(db => db.StringGetAsync(key, CommandFlags.None)).ReturnsAsync(cachedValue);

            var result = await _cacheService.GetCacheAsync<object>(key);

            Assert.NotNull(result);
        }
    }
}
