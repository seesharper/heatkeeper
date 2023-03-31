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
    }
}
