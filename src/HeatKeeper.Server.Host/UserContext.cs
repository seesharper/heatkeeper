using System.Security.Claims;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Users;
using Microsoft.AspNetCore.Http;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }
    public long Id
        => httpContextAccessor.HttpContext is not null ? long.Parse(httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Sid).Value) : BackgroundUser.Id;

    public string FirstName => httpContextAccessor.HttpContext is not null ? httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.GivenName).Value : BackgroundUser.FirstName;

    public string LastName => httpContextAccessor.HttpContext is not null ? httpContextAccessor.HttpContext.User.FindFirst(c => c.Type == ClaimTypes.Surname).Value : BackgroundUser.LastName;

    public string Email => httpContextAccessor.HttpContext is not null ? (httpContextAccessor.HttpContext.User?.FindFirst(c => c.Type == ClaimTypes.Email)?.Value) ?? "anonymous@tempuri.org" : BackgroundUser.Email;

    public bool IsAdmin => Role == "admin";

    public string Role => httpContextAccessor.HttpContext is not null ? (httpContextAccessor.HttpContext.User?.FindFirst(c => c.Type == ClaimTypes.Role)?.Value) ?? "anonymous" : BackgroundUser.Role;
}
