using System.Linq;
using System.Threading.Tasks;
using HeatKeeper.Server.CQRS;
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
        public async Task<IActionResult> Post([FromBody] CreateMeasurementRequest[] createMeasurementRequests)
        {
            await commandExecutor.ExecuteAsync(new CreateMissingSensorsCommand(createMeasurementRequests.Select(cmr => cmr.SensorId)));

            // var temperatureCommands = mapper.Map<TemperatureRequest[], ReportTemperatureCommand[]>(createMeasurementRequest);
            // await temperatureService.ReportTemperatures(temperatureCommands);
            return Ok();
        }
    }
}