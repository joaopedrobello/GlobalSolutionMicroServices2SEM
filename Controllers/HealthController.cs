using Microsoft.AspNetCore.Mvc;

namespace EnergyConsumptionMonitoring.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        [Route("health")]
        public IActionResult GetHealthStatus()
        {
            return Ok(new { status = "Service is running" });
        }
    }
}
