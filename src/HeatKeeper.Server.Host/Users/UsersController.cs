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
            Response.Cookies.Append("test", "test");
            Response.Cookies.Append("refreshToken2", "test", new Microsoft.AspNetCore.Http.CookieOptions
            {
                Expires = System.DateTime.UtcNow.AddDays(7),
                HttpOnly = true,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
                Secure = false
            });
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
        public async Task<User[]> Get([FromQuery] AllUsersQuery query)
            => await _queryExecutor.ExecuteAsync(query);

        [HttpPatch("password")]
        public async Task ChangePassword([FromBody] ChangePasswordCommand command)
            => await _commandExecutor.ExecuteAsync(command);

        [HttpGet("apikey")]
        public async Task<ApiKey> GetApiKey([FromQuery] ApiKeyQuery query)
            => await _queryExecutor.ExecuteAsync(query);
    }
}
