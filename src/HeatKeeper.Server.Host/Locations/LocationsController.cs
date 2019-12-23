using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Locations;
using HeatKeeper.Server.Users;
using HeatKeeper.Server.Zones;
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
        public async Task<ActionResult<CreateLocationResponse>> Post([FromBody] CreateLocationCommand command)
        {
            await commandExecutor.ExecuteAsync(command);
            return CreatedAtAction(nameof(Post), new CreateLocationResponse(command.Id));
        }

        [HttpGet]
        public async Task<Location[]> Get([FromQuery]GetAllLocationsQuery query)
            => await queryExecutor.ExecuteAsync(query);


        [HttpGet("{locationId}/zones")]
        public async Task<Zone[]> Zones([FromRoute]ZonesByLocationQuery query)
            => await queryExecutor.ExecuteAsync(query);

        [HttpPost("{locationId}/zones")]
        public async Task<IActionResult> Post([FromBodyAndRoute] CreateZoneCommand command)
        {
            await commandExecutor.ExecuteAsync(command);
            return CreatedAtAction(nameof(Post), new { id = command.Name });
        }




        [HttpPost("{locationId}/users")]
        public async Task<IActionResult> AddUser([FromBodyAndRoute]AddUserToLocationCommand command)
        {
            await commandExecutor.ExecuteAsync(command);
            return CreatedAtAction(nameof(AddUser), new AddUserLocationResponse(command.UserLocationId));
        }

        [HttpGet("{locationId}/users")]
        public async Task<User[]> GetUsers([FromRoute]UsersByLocationQuery query) =>
            await queryExecutor.ExecuteAsync(query);


        [HttpDelete("{locationId}/users/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RemoveUser([FromRoute]DeleteUserLocationCommand command)
        {
            await commandExecutor.ExecuteAsync(command);
            return NoContent();
        }
    }
}
