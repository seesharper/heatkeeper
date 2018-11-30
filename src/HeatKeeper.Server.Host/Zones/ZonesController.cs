using System.Threading.Tasks;
using HeatKeeper.Server.CQRS;
using HeatKeeper.Server.Zones;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.WebApi.Zones
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZonesController : ControllerBase
    {
        private readonly ICommandExecutor commandExecutor;

        public ZonesController(ICommandExecutor commandExecutor)
        {
            this.commandExecutor = commandExecutor;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateZoneRequest createZoneRequest)
        {
            var createZoneCommand = new CreateZoneCommand(createZoneRequest.Id, createZoneRequest.Description);
            await commandExecutor.ExecuteAsync(createZoneCommand);
            return CreatedAtAction(nameof(Post), new {id = createZoneRequest.Id});
        }
    }
}