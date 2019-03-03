using System.Linq;
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
        private readonly IQueryExecutor queryExecutor;
        private readonly IApiKeyProvider apiKeyProvider;
        private readonly ITokenProvider tokenProvider;

        public UsersController(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor, IApiKeyProvider apiKeyProvider)
        {
            this.commandExecutor = commandExecutor;
            this.queryExecutor = queryExecutor;
            this.apiKeyProvider = apiKeyProvider;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticateUserResponse>> Authenticate([FromBody]AuthenticateUserRequest request)
        {
            var query = new AuthenticatedUserQuery(request.Username, request.Password);
            var result = await queryExecutor.ExecuteAsync(query);
            return Ok(new AuthenticateUserResponse(result.Token, result.Id, result.Name, result.Email, result.IsAdmin));
        }

        [HttpPost()]
        public async Task<ActionResult<RegisterUserResponse>> Post([FromBody]RegisterUserRequest request)
        {
            var registerUserCommand = new RegisterUserCommand(request.Name, request.Email, request.IsAdmin, request.Password, request.ConfirmedPassword);
            await commandExecutor.ExecuteAsync(registerUserCommand);
            return Created(nameof(Post), new RegisterUserResponse(registerUserCommand.Id));
        }

        [HttpGet]
        public async Task<ActionResult<GetUserResponse[]>> Get()
        {
            var allUsersQuery = new AllUsersQuery();
            var result = await queryExecutor.ExecuteAsync(allUsersQuery);
            return result.Select(r => new GetUserResponse(r.Id, r.Name, r.Email, r.IsAdmin)).ToArray();
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