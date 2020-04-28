using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Sensors;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Host.Sensors
{
    [Route("api/[controller]")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly ICommandExecutor commandExecutor;

        public SensorsController(ICommandExecutor commandExecutor)
            => this.commandExecutor = commandExecutor;

        [HttpDelete("{sensorId}")]
        public async Task Delete([FromRoute]DeleteSensorCommand command)
            => await commandExecutor.ExecuteAsync(command);

        [HttpPatch("{sensorId}")]
        public async Task Patch([FromBodyAndRoute] UpdateSensorCommand command)
            => await commandExecutor.ExecuteAsync(command);
    }
}
