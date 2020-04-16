using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authentication;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Users;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Host.Users
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ICommandExecutor commandExecutor;
        private readonly IQueryExecutor queryExecutor;
        private readonly IApiKeyProvider apiKeyProvider;
        private readonly IUserContext userContext;

        public UsersController(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor, IApiKeyProvider apiKeyProvider, IUserContext userContext)
        {
            this.commandExecutor = commandExecutor;
            this.queryExecutor = queryExecutor;
            this.apiKeyProvider = apiKeyProvider;
            this.userContext = userContext;
        }

        [HttpPost("authenticate")]
        public async Task<AuthenticatedUser> Authenticate([FromBody]AuthenticatedUserQuery query)
            => await queryExecutor.ExecuteAsync(query);

        [HttpPost()]
        public async Task<ActionResult<RegisterUserResponse>> Post([FromBody]RegisterUserCommand command)
        {
            await commandExecutor.ExecuteAsync(command);
            return Created(nameof(Post), new RegisterUserResponse(command.UserId));
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete([FromRoute] DeleteUserCommand command)
        {
            await commandExecutor.ExecuteAsync(command);
            return NoContent();
        }

        [HttpPatch("{userId}")]
        public async Task PatchUser([FromBodyAndRoute]UpdateUserCommand command)
            => await commandExecutor.ExecuteAsync(command);

        [HttpPatch()]
        public async Task<ActionResult<RegisterUserResponse>> PatchCurrentUser([FromBodyAndRoute]UpdateCurrentUserCommand command)
        {
            command.UserId = userContext.Id;
            await commandExecutor.ExecuteAsync(command);
            return Ok();
        }

        [HttpGet]
        public async Task<User[]> Get([FromQuery]AllUsersQuery query)
            => await queryExecutor.ExecuteAsync(query);

        [HttpPatch("password")]
        public async Task ChangePassword([FromBody]ChangePasswordCommand command)
            => await commandExecutor.ExecuteAsync(command);

        [HttpGet("apikey")]
        public async Task<ApiKey> GetApiKey([FromQuery]ApiKeyQuery query)
            => await queryExecutor.ExecuteAsync(query);
    }
}
