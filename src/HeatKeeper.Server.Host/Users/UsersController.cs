using System.Linq;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authentication;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
        public async Task<ActionResult<AuthenticatedUserQueryResult>> Authenticate([FromBody]AuthenticatedUserQuery query)
            => Ok(await queryExecutor.ExecuteAsync(query));

        [HttpPost()]
        public async Task<ActionResult<RegisterUserResponse>> Post([FromBody]RegisterUserCommand command)
        {
            await commandExecutor.ExecuteAsync(command);
            return Created(nameof(Post), new RegisterUserResponse(command.UserId));
        }

        [HttpDelete("{userId}")]
        public async Task Delete([FromRoute] DeleteUserCommand command)
            => await commandExecutor.ExecuteAsync(command);

        /// <summary>
        /// Updates user information for any user. [AccessLevel:AdminRole]
        /// </summary>
        [HttpPatch("{userId}")]
        public async Task PatchUser([FromBodyAndRoute]UpdateUserCommand command)
            => await commandExecutor.ExecuteAsync(command);

        /// <summary>
        /// Updates user information for the current user. [AccessLevel:StandardRole]
        /// </summary>
        [HttpPatch()]
        public async Task<ActionResult<RegisterUserResponse>> PatchCurrentUser([FromBodyAndRoute]UpdateCurrentUserCommand command)
        {
            command.UserId = userContext.Id;
            await commandExecutor.ExecuteAsync(command);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<User[]>> Get([FromQuery]AllUsersQuery query) =>
            Ok(await queryExecutor.ExecuteAsync(query));

        /// <summary>
        /// Changes the password for the current user.
        /// </summary>
        [HttpPatch("password")]
        public async Task ChangePassword([FromBody]ChangePasswordCommand command)
            => await commandExecutor.ExecuteAsync(command);

        /// <summary>
        /// Creates an API key to be used when posting measurements.
        /// </summary>
        [HttpGet("apikey")]
        public async Task<ApiKey> GetApiKey([FromQuery]ApiKeyQuery query)
            => await queryExecutor.ExecuteAsync(query);
    }



}
