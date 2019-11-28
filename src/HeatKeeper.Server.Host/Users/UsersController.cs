using System.Linq;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Users;
using Microsoft.AspNetCore.Authorization;
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

        public UsersController(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor, IApiKeyProvider apiKeyProvider)
        {
            this.commandExecutor = commandExecutor;
            this.queryExecutor = queryExecutor;
            this.apiKeyProvider = apiKeyProvider;
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticateUserResponse>> Authenticate([FromBody]AuthenticateUserRequest request)
        {
            var query = new AuthenticatedUserQuery(request.Email, request.Password);
            var result = await queryExecutor.ExecuteAsync(query);
            return Ok(new AuthenticateUserResponse(result.Token, result.Id, result.Email, result.FirstName, result.LastName, result.IsAdmin));
        }

        [HttpPost()]
        public async Task<ActionResult<RegisterUserResponse>> Post([FromBody]RegisterUserRequest request)
        {
            var registerUserCommand = new RegisterUserCommand(request.Email, request.FirstName, request.LastName, request.IsAdmin, request.Password, request.ConfirmedPassword);
            await commandExecutor.ExecuteAsync(registerUserCommand);
            return Created(nameof(Post), new RegisterUserResponse(registerUserCommand.Id));
        }

        [HttpGet]
        public async Task<ActionResult<User[]>> Get()
        {
            return Ok(await queryExecutor.ExecuteAsync(new AllUsersQuery()));
        }

        /// <summary>
        /// Changes the password for the current user.
        /// </summary>
        [HttpPatch("password")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordRequest request)
        {
            var changePasswordCommand = new ChangePasswordCommand(request.OldPassword, request.NewPassword, request.ConfirmedPassword);
            await commandExecutor.ExecuteAsync(changePasswordCommand);
            return Ok();
        }

        /// <summary>
        /// Creates an API key to be used when posting measurements.
        /// </summary>
        [HttpGet("apikey")]
        public ActionResult<GetApiKeyResponse> GetApiKey()
        {
            var apiKey = apiKeyProvider.CreateApiKey();
            return new GetApiKeyResponse(apiKey.Token);
        }
    }
}
