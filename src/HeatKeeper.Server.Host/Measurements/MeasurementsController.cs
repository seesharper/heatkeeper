using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Measurements
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeasurementsController(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] MeasurementCommand[] createMeasurementCommands)
        {
            await commandExecutor.ExecuteAsync(createMeasurementCommands);
            return Ok();
        }
    }
}
