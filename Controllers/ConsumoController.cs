using GlobalSolution.Models;
using GlobalSolution.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GlobalSolution.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsumoController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;
        private readonly CacheService _cacheService;

        public ConsumoController(MongoDbService mongoDbService, CacheService cacheService)
        {
            _mongoDbService = mongoDbService;
            _cacheService = cacheService;
        }

        [HttpPost]
        [Route("consumo")]
        public async Task<IActionResult> RegistrarConsumo([FromBody] ConsumoModel consumo)
        {
            if (consumo == null || consumo.PowerUsage <= 0)
            {
                return BadRequest("Dados de consumo inválidos.");
            }

            consumo.Timestamp = DateTime.Now;

            try
            {
                await _mongoDbService.InsertConsumoAsync(consumo);
                await _cacheService.SetCacheAsync($"consumo:{consumo.Device}", consumo); // Cache após inserir no MongoDB
                return CreatedAtAction(nameof(RegistrarConsumo), new { id = consumo.Id }, consumo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao salvar o consumo: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("consumo")]
        public async Task<IActionResult> ObterConsumos()
        {
            const string cacheKey = "consumos";

            var cachedConsumos = await _cacheService.GetCacheAsync<List<ConsumoModel>>(cacheKey);
            if (cachedConsumos != null)
            {
                return StatusCode(200, cachedConsumos);
            }

            try
            {
                var consumos = await _mongoDbService.GetConsumosAsync();
                if (consumos.Count == 0)
                {
                    return NotFound("Nenhum dado de consumo encontrado.");
                }

                await _cacheService.SetCacheAsync(cacheKey, consumos);

                return StatusCode(200, consumos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao recuperar consumos: {ex.Message}");
            }
        }
    }
}
