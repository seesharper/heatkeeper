using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Measurements
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateMeasurementRequest[] createMeasurementRequest)
        {
            return Ok();
        }
    }
}