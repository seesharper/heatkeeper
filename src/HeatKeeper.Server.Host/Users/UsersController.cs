using System.Threading.Tasks;
using HeatKeeper.Server.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HeatKeeper.Server.Host.Users
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
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

        [HttpPost]
        [Authorize("Admin")]
        public async Task<ActionResult<AuthenticateUserResponse>> Post([FromBody]CreateUserCommand request)
        {
            // var token = await userService.Authenticate(request.Name, request.Password);
            // if (token == null)
            // {
            //     return BadRequest(new { message = "Username or password is incorrect" });
            // }

            // return Ok(new AuthenticateUserResponse(token));
        }
    }
}