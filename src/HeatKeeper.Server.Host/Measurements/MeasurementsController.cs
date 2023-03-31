using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Measurements
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeasurementsController : ControllerBase
    {
        private readonly ICommandExecutor _commandExecutor;
        private readonly IQueryExecutor _queryExecutor;

        public MeasurementsController(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor)
        {
            _commandExecutor = commandExecutor;
            _queryExecutor = queryExecutor;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MeasurementCommand[] createMeasurementCommands)
        {
            await _commandExecutor.ExecuteAsync(createMeasurementCommands);
            return Ok();
        }

        [HttpGet("latest")]
        public async Task<Measurement[]> GetLatestMeasurements([FromQuery] LatestTenMeasurementsQuery query)
            => await _queryExecutor.ExecuteAsync(query);
    }
}
