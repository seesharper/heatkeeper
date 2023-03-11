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
        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;

        public ZonesController(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor)
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
        }

        [HttpPatch("{zoneId}")]
        public async Task Patch([FromBodyAndRoute] UpdateZoneCommand updateZoneCommand)
           => await _commandExecutor.ExecuteAsync(updateZoneCommand);

        [HttpDelete("{zoneId}")]
        public async Task Delete([FromRoute] DeleteZoneCommand deleteZoneCommand)
            => await _commandExecutor.ExecuteAsync(deleteZoneCommand);

        [HttpGet("{zoneId}")]
        public async Task<ZoneDetails> GetZoneDetails([FromRoute] ZoneDetailsQuery query)
            => await _queryExecutor.ExecuteAsync(query);

        [HttpGet("{zoneId}/sensors")]
        public async Task<Sensor[]> GetSensors([FromRoute] SensorsByZoneQuery query)
            => await _queryExecutor.ExecuteAsync(query);

        [HttpPost("{zoneId}/sensors")]
        public async Task AddSensorToZone([FromBodyAndRoute] AddSensorToZoneCommand command)
            => await _commandExecutor.ExecuteAsync(command);

        [HttpDelete("{zoneId}/sensors")]
        public async Task RemoveSensorFromZone([FromBodyAndRoute] RemoveSensorFromZoneCommand command)
            => await _commandExecutor.ExecuteAsync(command);
    }
}
