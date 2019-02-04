using System.Threading.Tasks;
using HeatKeeper.Abstractions.CQRS;
using HeatKeeper.Server.Locations;
using HeatKeeper.Server.Mapping;
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

        [HttpPost("adduser")]
        public async Task<IActionResult> AddUser([FromBody] AddUserRequest request)
        {
            var addUserCommand = new AddUserCommand(request.UserId, request.LocationId);
            await commandExecutor.ExecuteAsync(addUserCommand);
            return CreatedAtAction(nameof(AddUser),new AddUserResponse(request.LocationId));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await queryExecutor.ExecuteAsync(new GetAllLocationsQuery());
            return Ok(result);
        }
    }
}