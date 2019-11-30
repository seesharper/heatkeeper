using System.Linq;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
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


        /// <summary>
        /// Updates user information for any user. [AccessLevel:AdminRole]
        /// </summary>
        [HttpPatch("{userId}")]
        public async Task<IActionResult> PatchUser([FromBodyAndRoute]UpdateUserCommand command)
        {
            await commandExecutor.ExecuteAsync(command);
            return Ok();
        }

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
        public async Task<ActionResult<User[]>> Get() =>
            Ok(await queryExecutor.ExecuteAsync(new AllUsersQuery()));

        /// <summary>
        /// Changes the password for the current user.
        /// </summary>
        [HttpPatch("password")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordCommand command)
        {
            await commandExecutor.ExecuteAsync(command);
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
