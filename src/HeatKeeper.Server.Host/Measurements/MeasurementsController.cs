using System.Linq;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Sensors;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Measurements
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeasurementsController : ControllerBase
    {
        private readonly ICommandExecutor commandExecutor;
        private readonly IQueryExecutor queryExecutor;

        public MeasurementsController(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor)
        {
            this.commandExecutor = commandExecutor;
            this.queryExecutor = queryExecutor;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MeasurementCommand[] createMeasurementCommands)
        {
            await commandExecutor.ExecuteAsync(new CreateMissingSensorsCommand(createMeasurementCommands.Select(cmr => cmr.SensorId)));
            await commandExecutor.ExecuteAsync(createMeasurementCommands);
            return Ok();
        }

        [HttpGet("latest")]
        public async Task<Measurement[]> GetLatestMeasurements([FromQuery] LatestTenMeasurementsQuery query)
            => await queryExecutor.ExecuteAsync(query);
    }
}
