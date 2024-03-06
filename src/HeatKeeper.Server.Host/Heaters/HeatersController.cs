using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Heaters;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Host.Sensors
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeatersController(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor) : ControllerBase
    {
        [HttpDelete("{heaterId}")]
        public async Task Delete([FromRoute] DeleteHeaterCommand command)
            => await commandExecutor.ExecuteAsync(command);

        [HttpPatch("{heaterId}")]
        public async Task Patch([FromBodyAndRoute] UpdateHeaterCommand command)
            => await commandExecutor.ExecuteAsync(command);

        [HttpGet("{heaterId}")]
        public async Task<HeaterDetails> GetHeaterDetails([FromRoute] HeaterDetailsQuery query)
            => await queryExecutor.ExecuteAsync(query);      
    }
}
