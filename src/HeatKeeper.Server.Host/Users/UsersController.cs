using System.Threading.Tasks;
using HeatKeeper.Abstractions.CQRS;
using HeatKeeper.Server.Mapping;
using HeatKeeper.Server.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Host.Users
{
    [Authorize(Roles = "admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ICommandExecutor commandExecutor;
        private readonly IAuthenticationManager authenticationManager;
        private readonly IApiKeyProvider apiKeyProvider;
        private readonly ITokenProvider tokenProvider;

        public UsersController(ICommandExecutor commandExecutor, IAuthenticationManager authenticationManager, IApiKeyProvider apiKeyProvider)
        {
            this.commandExecutor = commandExecutor;
            this.authenticationManager = authenticationManager;
            this.apiKeyProvider = apiKeyProvider;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticateUserResponse>> Authenticate([FromBody]AuthenticateUserRequest request)
        {
            var authenticationResult = await authenticationManager.Authenticate(request.Name, request.Password);
            if (authenticationResult.IsAuthenticated)
            {
                return Ok(new AuthenticateUserResponse(authenticationResult.Token));
            }

            return BadRequest(new { message = "Username or password is incorrect" });
        }

        [HttpPost()]
        public async Task<ActionResult<RegisterUserResponse>> Post([FromBody]CreateUserRequest request)
        {
            var registerUserCommand = new RegisterUserCommand(request.Name, request.Email, request.Password, request.IsAdmin);
            await commandExecutor.ExecuteAsync(registerUserCommand);
            return Created(nameof(Post), new RegisterUserResponse(registerUserCommand.Id));
        }

        [HttpGet("apikey")]
        public ActionResult<GetApiKeyResponse> GetApiKey()
        {
            var apiKey = apiKeyProvider.CreateApiKey();
            return new GetApiKeyResponse(apiKey.Token);
        }
    }
}