using System.Linq;
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
        private readonly IQueryExecutor queryExecutor;

        public ZonesController(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor)
        {
            this.commandExecutor = commandExecutor;
            this.queryExecutor = queryExecutor;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateZoneRequest createZoneRequest)
        {
            var createZoneCommand = new CreateZoneCommand(createZoneRequest.Name, createZoneRequest.Description, createZoneRequest.Location);
            await commandExecutor.ExecuteAsync(createZoneCommand);
            return CreatedAtAction(nameof(Post), new { id = createZoneRequest.Name });
        }

        [HttpGet]
        public async Task<IActionResult> Get(string location)
        {
            var result = await queryExecutor.ExecuteAsync(new GetAllZonesQuery(location));
            var response = result.Select(zrq => new ZoneResponse(zrq.Id, zrq.Description)).ToArray();
            return Ok(response);
        }
    }
}