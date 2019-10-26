using System.Security.Claims;
using HeatKeeper.Server.Users;
using Microsoft.AspNetCore.Http;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }
    public long Id => long.Parse(httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Sid).Value);

    public string Name => httpContextAccessor.HttpContext.User.Identity.Name;

    public string Email => httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Email).Value;

    public bool IsAdmin => bool.Parse(httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Email).Value);
}
