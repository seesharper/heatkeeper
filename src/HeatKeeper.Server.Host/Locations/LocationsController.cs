using System.Linq;
using System.Threading.Tasks;
using HeatKeeper.Abstractions.CQRS;
using HeatKeeper.Server.Host.Zones;
using HeatKeeper.Server.Locations;
using HeatKeeper.Server.Mapping;
using HeatKeeper.Server.Zones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Host.Locations
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class LocationsController : ControllerBase
    {
        private readonly IQueryExecutor queryExecutor;
        private readonly ICommandExecutor commandExecutor;
        private readonly IMapper mapper;

        public LocationsController(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor, IMapper mapper)
        {
            this.queryExecutor = queryExecutor;
            this.commandExecutor = commandExecutor;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<CreateLocationResponse>> Post([FromBody] CreateLocationRequest request)
        {
            var command = new CreateLocationCommand(request.Name, request.Description);
            await commandExecutor.ExecuteAsync(command);
            return CreatedAtAction(nameof(Post), new CreateLocationResponse(command.Id));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await queryExecutor.ExecuteAsync(new GetAllLocationsQuery());
            return Ok(result);
        }

        [HttpGet("{locationId}/zones")]
        public async Task<ActionResult<ZoneResponse[]>> Zones(long locationId)
        {
            var zonesByLocationQuery = new ZonesByLocationQuery(locationId);
            var result = await queryExecutor.ExecuteAsync(zonesByLocationQuery);
            var response = result.Select(zrq => new ZoneResponse(zrq.Id, zrq.Name, zrq.Description)).ToArray();
            return Ok(response);
        }

        [HttpPost("{locationId}/zones")]
        public async Task<IActionResult> Post(long locationId, [FromBody] CreateZoneRequest createZoneRequest)
        {
            var createZoneCommand = new InsertZoneCommand(createZoneRequest.Name, createZoneRequest.Description, locationId);
            await commandExecutor.ExecuteAsync(createZoneCommand);
            return CreatedAtAction(nameof(Post), new { id = createZoneRequest.Name });
        }

        [HttpPost("users")]
        public async Task<IActionResult> AddUser([FromBody] AddUserLocationRequest request)
        {
            var addUserCommand = new InsertUserLocationCommand(request.UserId, request.LocationId);
            await commandExecutor.ExecuteAsync(addUserCommand);
            return CreatedAtAction(nameof(AddUser),new AddUserLocationResponse(addUserCommand.UserLocationId));
        }

        [HttpDelete("users")]
        public async Task<IActionResult> RemoveUser([FromBody] RemoveUserRequest request)
        {
            var removeUserCommand = new DeleteUserLocationCommand(request.UserLocationId);
            await commandExecutor.ExecuteAsync(removeUserCommand);
            return Ok();
        }
    }
}