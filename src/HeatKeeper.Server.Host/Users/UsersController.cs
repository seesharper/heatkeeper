using System.Net;
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
            var command = new AuthenticateCommand(request.Username, request.Password);
            await commandExecutor.ExecuteAsync(command);
            return Ok(new AuthenticateUserResponse(command.Token));
        }

        [HttpPost()]
        public async Task<ActionResult<RegisterUserResponse>> Post([FromBody]CreateUserRequest request)
        {
            var registerUserCommand = new RegisterUserCommand(request.Name, request.Email, request.Password, request.IsAdmin);
            await commandExecutor.ExecuteAsync(registerUserCommand);
            return Created(nameof(Post), new RegisterUserResponse(registerUserCommand.Id));
        }

        [HttpPatch("password")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordRequest request)
        {
            var changePasswordCommand = new ChangePasswordCommand(request.OldPassword,request.NewPassword, request.ConfirmedPassword);
            await commandExecutor.ExecuteAsync(changePasswordCommand);
            return Ok();
        }


        [HttpGet("apikey")]
        public ActionResult<GetApiKeyResponse> GetApiKey()
        {
            var apiKey = apiKeyProvider.CreateApiKey();
            return new GetApiKeyResponse(apiKey.Token);
        }
    }
}