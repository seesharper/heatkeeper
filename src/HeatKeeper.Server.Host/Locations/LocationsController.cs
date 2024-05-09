using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Locations;
using HeatKeeper.Server.Programs;
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

        // [HttpPost]
        // public async Task<IActionResult> Post([FromBody] CreateLocationCommand command)
        // {
        //     await commandExecutor.ExecuteAsync(command);
        //     return CreatedAtAction(nameof(Post), new ResourceId(command.Id));
        // }

        // [HttpDelete("{locationId}")]
        // public async Task Delete([FromRoute] DeleteLocationCommand command)
        //     => await commandExecutor.ExecuteAsync(command);

        [HttpPatch("{locationId}")]
        public async Task Patch([FromBodyAndRoute] UpdateLocationCommand updateLocationCommand)
            => await commandExecutor.ExecuteAsync(updateLocationCommand).ConfigureAwait(false);

      

        [HttpGet]
        public async Task<Location[]> Get([FromQuery] GetAllLocationsQuery query)
        {
            return await queryExecutor.ExecuteAsync(query);
        }

        [HttpGet("{locationId}/zones")]
        public async Task<ZoneInfo[]> Zones([FromRoute] ZonesByLocationQuery query)
            => await queryExecutor.ExecuteAsync(query);

        [HttpPost("{locationId}/zones")]
        public async Task<IActionResult> Post([FromBodyAndRoute] CreateZoneCommand command)
        {
            await commandExecutor.ExecuteAsync(command);
            return CreatedAtAction(nameof(Post), new ResourceId(command.ZoneId));
        }

        [HttpPost("{locationId}/users")]
        public async Task<IActionResult> AddUser([FromBodyAndRoute] AddUserToLocationCommand command)
        {
            await commandExecutor.ExecuteAsync(command);
            return Ok();
        }

        [HttpPost("{locationId}/programs")]
        public async Task<IActionResult> AddProgram([FromBodyAndRoute] CreateProgramCommand command)
        {
            await commandExecutor.ExecuteAsync(command);
            return CreatedAtAction(nameof(AddProgram), new ResourceId(command.ProgramId));
        }

        [HttpGet("{locationId}/programs")]
        public async Task<Server.Programs.Program[]> Programs([FromRoute] ProgramsByLocationQuery query)
            => await queryExecutor.ExecuteAsync(query);


        [HttpGet("{locationId}/users")]
        public async Task<UserInfo[]> GetUsers([FromRoute] UsersByLocationQuery query) =>
            await queryExecutor.ExecuteAsync(query);


        [HttpDelete("{locationId}/users/{userId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task RemoveUser([FromRoute] DeleteUserLocationCommand command)
            => await commandExecutor.ExecuteAsync(command);

        [HttpGet("{locationId}/temperatures")]
        public async Task<LocationTemperature[]> GetTemperatures([FromRoute] LocationTemperaturesQuery query)
            => await queryExecutor.ExecuteAsync(query);
    }
}
