using System.Security.Claims;
using HeatKeeper.Server.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace HeatKeeper.Server.Users.Api;

public class AuthenticatedUserQueryHandler(IPasswordManager passwordManager, ITokenProvider tokenProvider, IQueryExecutor queryExecutor, IHttpContextAccessor httpContextAccessor) : IQueryHandler<AuthenticatedUserQuery, AuthenticatedUser>
{
    public async Task<AuthenticatedUser> HandleAsync(AuthenticatedUserQuery query, CancellationToken cancellationToken = default)
    {
        var user = await queryExecutor.ExecuteAsync(new GetUserQuery(query.Email));

        if (user == null || !passwordManager.VerifyPassword(query.Password, user.HashedPassword))
        {
            throw new AuthenticationFailedException("Invalid username/password");
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.GivenName, user.FirstName),
            new Claim(ClaimTypes.Surname, user.LastName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.IsAdmin ? "admin" : "user"),
            new Claim(ClaimTypes.Sid, user.Id.ToString())
        };

        var token = tokenProvider.CreateToken(claims, DateTime.UtcNow.AddHours(1));

        var authProperties = new AuthenticationProperties
        {
            AllowRefresh = true,
            // Refreshing the authentication session should be allowed.

            //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
            // The time at which the authentication ticket expires. A 
            // value set here overrides the ExpireTimeSpan option of 
            // CookieAuthenticationOptions set with AddCookie.

            IsPersistent = true,
            // Whether the authentication session is persisted across 
            // multiple requests. When used with cookies, controls
            // whether the cookie's lifetime is absolute (matching the
            // lifetime of the authentication ticket) or session-based.

            //IssuedUtc = <DateTimeOffset>,
            // The time at which the authentication ticket was issued.

            //RedirectUri = <string>
            // The full path or absolute URI to be used as an http 
            // redirect response value.


        };

        ClaimsIdentity ct = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        ClaimsPrincipal cp = new ClaimsPrincipal(ct);
        await httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, cp, authProperties);

        return new AuthenticatedUser(token, user.Id, user.Email, user.FirstName, user.LastName, user.IsAdmin);
    }
}

[RequireAnonymousRole]
[Post("/api/users/authenticate")]
public record AuthenticatedUserQuery(string Email, string Password) : IQuery<AuthenticatedUser>;

public record AuthenticatedUser(string Token, long Id, string Email, string FirstName, string LastName, bool IsAdmin);
