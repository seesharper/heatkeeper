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

    public string FirstName => httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.GivenName).Value;

    public string LastName => httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Surname).Value;

    public string Email => (httpContextAccessor.HttpContext.User?.FindFirst(c => c.Type == ClaimTypes.Email)?.Value) ?? "anonymous@tempuri.org";

    public bool IsAdmin => bool.Parse(httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Email).Value);

    public string Role => (httpContextAccessor.HttpContext.User?.FindFirst(c => c.Type == ClaimTypes.Role)?.Value) ?? "anonymous";
}
