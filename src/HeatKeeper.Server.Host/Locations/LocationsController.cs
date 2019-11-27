using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Host.Zones;
using HeatKeeper.Server.Locations;
using HeatKeeper.Server.Users;
using HeatKeeper.Server.Zones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Host.Locations
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly IQueryExecutor queryExecutor;
        private readonly ICommandExecutor commandExecutor;

        public LocationsController(IQueryExecutor queryExecutor, ICommandExecutor commandExecutor)
        {
            this.queryExecutor = queryExecutor;
            this.commandExecutor = commandExecutor;
        }

        [HttpPost]
        public async Task<ActionResult<CreateLocationResponse>> Post([FromBody] CreateLocationRequest request)
        {
            var command = new CreateLocationCommand(request.Name, request.Description);
            await commandExecutor.ExecuteAsync(command);
            return CreatedAtAction(nameof(Post), new CreateLocationResponse(command.Id));
        }

        [HttpGet]
        public async Task<ActionResult<Location[]>> Get() =>
            Ok(await queryExecutor.ExecuteAsync(new GetAllLocationsQuery()));

        [HttpGet("{locationId}/zones")]
        public async Task<ActionResult<Zone[]>> Zones(long locationId) =>
            Ok(await queryExecutor.ExecuteAsync(new ZonesByLocationQuery(locationId)));


        [HttpPost("{locationId}/zones")]
        public async Task<IActionResult> Post(long locationId, [FromBody] CreateZoneRequest createZoneRequest)
        {
            var createZoneCommand = new InsertZoneCommand(createZoneRequest.Name, createZoneRequest.Description, locationId);
            await commandExecutor.ExecuteAsync(createZoneCommand);
            return CreatedAtAction(nameof(Post), new { id = createZoneRequest.Name });
        }

        [HttpPost("{locationId}/users")]
        public async Task<IActionResult> AddUser(long locationId, [FromBody] AddUserLocationRequest request)
        {
            var addUserCommand = new InsertUserLocationCommand(request.UserId, locationId);
            await commandExecutor.ExecuteAsync(addUserCommand);
            return CreatedAtAction(nameof(AddUser), new AddUserLocationResponse(addUserCommand.UserLocationId));
        }

        [HttpGet("{locationId}/users")]
        public async Task<ActionResult<User[]>> GetUsers(long locationId) =>
            Ok(await queryExecutor.ExecuteAsync(new UsersByLocationQuery(locationId)));


        [HttpDelete("{locationId}/users/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RemoveUser(long locationId, long userId)
        {
            await commandExecutor.ExecuteAsync(new DeleteUserLocationCommand(locationId, userId));
            return NoContent();
        }
    }
}
