using System.Linq;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Sensors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Measurements
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeasurementsController : ControllerBase
    {
        private readonly ICommandExecutor commandExecutor;

        public MeasurementsController(ICommandExecutor commandExecutor)
        {
            this.commandExecutor = commandExecutor;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateMeasurementCommand[] createMeasurementCommands)
        {
            await commandExecutor.ExecuteAsync(new CreateMissingSensorsCommand(createMeasurementCommands.Select(cmr => cmr.SensorId)));
            await commandExecutor.ExecuteAsync(createMeasurementCommands);
            return Ok();
        }
    }
}
