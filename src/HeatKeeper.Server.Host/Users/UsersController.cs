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
        private readonly ICommandExecutor _commandExecutor;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IApiKeyProvider _apiKeyProvider;
        private readonly IUserContext _userContext;

        public UsersController(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor, IApiKeyProvider apiKeyProvider, IUserContext userContext)
        {
            _commandExecutor = commandExecutor;
            _queryExecutor = queryExecutor;
            _apiKeyProvider = apiKeyProvider;
            _userContext = userContext;
        }

        [HttpPost("authenticate")]
        public async Task<AuthenticatedUser> Authenticate([FromBody] AuthenticatedUserQuery query)
        {
            return await _queryExecutor.ExecuteAsync(query);
        }

        [HttpPost()]
        public async Task<ActionResult<ResourceId>> Post([FromBody] RegisterUserCommand command)
        {
            await _commandExecutor.ExecuteAsync(command);
            return Created(nameof(Post), new ResourceId(command.UserId));
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete([FromRoute] DeleteUserCommand command)
        {
            await _commandExecutor.ExecuteAsync(command);
            return NoContent();
        }

        [HttpPatch("{userId}")]
        public async Task PatchUser([FromBodyAndRoute] UpdateUserCommand command)
            => await _commandExecutor.ExecuteAsync(command);

        [HttpPatch()]
        public async Task<ActionResult<ResourceId>> PatchCurrentUser([FromBodyAndRoute] UpdateCurrentUserCommand command)
        {
            command.UserId = _userContext.Id;
            await _commandExecutor.ExecuteAsync(command);
            return Ok();
        }

        [HttpGet]
        public async Task<UserInfo[]> Get([FromQuery] AllUsersQuery query)
            => await _queryExecutor.ExecuteAsync(query);

        [HttpGet("{userId}")]
        public async Task<UserDetails> Get([FromRoute] GetUserDetailsQuery query)
            => await _queryExecutor.ExecuteAsync(query);

        [HttpGet("{userId}/locations-access")]
        public async Task<UserLocationAccess[]> GetUserLocationsAccess([FromRoute] GetUserLocationsAccessQuery query)
            => await _queryExecutor.ExecuteAsync(query);

        [HttpPatch("password")]
        public async Task ChangePassword([FromBody] ChangePasswordCommand command)
            => await _commandExecutor.ExecuteAsync(command);

        [HttpPatch("{userId}/assignLocation")]
        public async Task AssignLocation([FromBodyAndRoute] AssignLocationToUserCommand command)
            => await _commandExecutor.ExecuteAsync(command);

        [HttpPatch("{userId}/removeLocation")]
        public async Task RemoveLocation([FromBodyAndRoute] RemoveLocationFromUserCommand command)
           => await _commandExecutor.ExecuteAsync(command);

        [HttpGet("apikey")]
        public async Task<ApiKey> GetApiKey([FromQuery] ApiKeyQuery query)
            => await _queryExecutor.ExecuteAsync(query);

        [HttpPost("refresh-access-token")]
        public async Task<string> RefreshToken()
        {
            var refreshToken = Request.Cookies["refresh-token"];

            // 1  . Check if refresh token is valid
            // 2. If valid, return new access token
            // 3. If not valid, return 401


            // if (refreshToken == null)
            // {
            //     throw new AuthenticationFailedException("No refresh token found");
            // }
            return string.Empty;


            //return await _apiKeyProvider.RefreshToken(refreshToken);
        }
    }
}
