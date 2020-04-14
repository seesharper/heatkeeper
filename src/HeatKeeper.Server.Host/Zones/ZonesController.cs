using System.Net;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Sensors;
using HeatKeeper.Server.Zones;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Host.Zones
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZonesController : ControllerBase
    {
        private readonly IQueryExecutor queryExecutor;
        private readonly ICommandExecutor commandExecutor;

        public ZonesController(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor)
        {
            this.queryExecutor = queryExecutor;
            this.commandExecutor = commandExecutor;
        }

        [HttpPatch("{zoneId}")]
        public async Task Patch([FromBodyAndRoute] UpdateZoneCommand updateZoneCommand)
           => await commandExecutor.ExecuteAsync(updateZoneCommand).ConfigureAwait(false);


        [HttpGet("{zoneId}")]
        public async Task<ZoneDetails> GetZoneDetails([FromRoute]ZoneDetailsQuery query)
        {
            return await queryExecutor.ExecuteAsync(query);
        }

        [HttpGet("{zoneId}/sensors")]
        public async Task<Sensor[]> GetSensors([FromRoute]SensorsByZoneQuery query)
        {
            return await queryExecutor.ExecuteAsync(query);
        }

        [HttpPost("{zoneId}/sensors")]
        public async Task<IActionResult> AddSensorToZone([FromBodyAndRoute] AddSensorToZoneCommand command)
        {
            await commandExecutor.ExecuteAsync(command);
            return new StatusCodeResult((int)HttpStatusCode.Created);
        }

        [HttpDelete("{zoneId}/sensors")]
        public async Task<IActionResult> RemoveSensorFromZone([FromBodyAndRoute] RemoveSensorFromZoneCommand command)
        {
            await commandExecutor.ExecuteAsync(command);
            return NoContent();
        }
    }
}
