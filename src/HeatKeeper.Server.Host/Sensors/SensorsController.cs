using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Sensors;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Host.Sensors
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly IQueryExecutor _queryExecutor;

        public SensorsController(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor)
        {
            _commandExecutor = commandExecutor;
            _queryExecutor = queryExecutor;
        }

        [HttpDelete("{sensorId}")]
        public async Task Delete([FromRoute] DeleteSensorCommand command)
            => await _commandExecutor.ExecuteAsync(command);

        [HttpPatch("{sensorId}")]
        public async Task Patch([FromBodyAndRoute] UpdateSensorCommand command)
            => await _commandExecutor.ExecuteAsync(command);

        [HttpGet("DeadSensors")]
        public async Task<DeadSensor[]> GetDeadSensors([FromQuery] DeadSensorsQuery query)
            => await _queryExecutor.ExecuteAsync(new DeadSensorsQuery());

        [HttpGet()]
        public async Task<UnassignedSensorInfo[]> GetUnassignedSensors()
            => await _queryExecutor.ExecuteAsync(new UnassignedSensorsQuery());

        [HttpGet("{sensorId}")]
        public async Task<SensorDetails> GetSensorDetails([FromRoute] SensorDetailsQuery query)
            => await _queryExecutor.ExecuteAsync(query);

        [HttpPatch("{sensorId}/assignZone")]
        public async Task AssignSensorToZone([FromBodyAndRoute] AssignZoneToSensorCommand command)
            => await _commandExecutor.ExecuteAsync(command);

        [HttpPatch("{sensorId}/removeZone")]
        public async Task RemoveSensorFromZone([FromBodyAndRoute] RemoveZoneFromSensorCommand command)
            => await _commandExecutor.ExecuteAsync(command);

    }
}
