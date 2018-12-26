using System.Threading.Tasks;
using HeatKeeper.Server.CQRS;
using HeatKeeper.Server.Locations;
using HeatKeeper.Server.Mapping;
using heatkeeper_server.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Host.Locations
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly IQueryExecutor queryExecutor;
        private readonly ICommandExecutor commandExecutor;
        private readonly IMapper mapper;

        public LocationsController(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor, IMapper mapper, DisposeTest d)
        {
            this.queryExecutor = queryExecutor;
            this.commandExecutor = commandExecutor;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateLocationRequest createLocationRequest)
        {
            var command = mapper.Map<CreateLocationRequest,CreateLocationCommand>(createLocationRequest);
            await commandExecutor.ExecuteAsync(command);
            return CreatedAtAction(nameof(Post), new { id = createLocationRequest.Name });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await queryExecutor.ExecuteAsync(new GetAllLocationsQuery());
            return Ok(result);
        }
    }
}