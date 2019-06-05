using System.Linq;
using System.Threading.Tasks;
using HeatKeeper.Abstractions.CQRS;
using HeatKeeper.Server.Mapping;
using HeatKeeper.Server.Sensors;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Measurements
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeasurementsController : ControllerBase
    {
        private readonly ICommandExecutor commandExecutor;
        private readonly IMapper mapper;

        public MeasurementsController(ICommandExecutor commandExecutor, IMapper mapper)
        {
            this.commandExecutor = commandExecutor;
            this.mapper = mapper;
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