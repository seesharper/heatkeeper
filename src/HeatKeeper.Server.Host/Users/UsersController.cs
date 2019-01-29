using System.Threading.Tasks;
using HeatKeeper.Server.CQRS;
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
        private readonly IUserService userService;
        private readonly ICommandExecutor commandExecutor;
        private readonly IMapper mapper;

        public UsersController(IUserService userService, ICommandExecutor commandExecutor, IMapper mapper)
        {
            this.userService = userService;
            this.commandExecutor = commandExecutor;
            this.mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticateUserResponse>> Authenticate([FromBody]AuthenticateUserRequest request)
        {
            var token = await userService.Authenticate(request.Name, request.Password);
            if (token == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            return Ok(new AuthenticateUserResponse(token));
        }

        [HttpPost()]
        public async Task<ActionResult> Post([FromBody]CreateUserRequest request)
        {
            var newUser = mapper.Map<CreateUserRequest, NewUser>(request);
            await userService.CreateUser(newUser);
            return Created(nameof(Post),new { id = request.Name });
        }
    }
}