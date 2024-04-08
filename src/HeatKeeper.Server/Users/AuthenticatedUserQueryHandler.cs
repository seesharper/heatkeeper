using System.Collections.Generic;
using System.Security.Claims;
using HeatKeeper.Server.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace HeatKeeper.Server.Users
{
    public class AuthenticatedUserQueryHandler : IQueryHandler<AuthenticatedUserQuery, AuthenticatedUser>
    {
        private readonly IPasswordManager _passwordManager;
        private readonly ITokenProvider _tokenProvider;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticatedUserQueryHandler(IPasswordManager passwordManager, ITokenProvider tokenProvider, IQueryExecutor queryExecutor, IHttpContextAccessor httpContextAccessor)
        {
            _passwordManager = passwordManager;
            _tokenProvider = tokenProvider;
            _queryExecutor = queryExecutor;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthenticatedUser> HandleAsync(AuthenticatedUserQuery query, CancellationToken cancellationToken = default)
        {
            var user = await _queryExecutor.ExecuteAsync(new GetUserQuery(query.Email));

            if (user == null || !_passwordManager.VerifyPassword(query.Password, user.HashedPassword))
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

            var token = _tokenProvider.CreateToken(claims, DateTime.UtcNow.AddHours(1));

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
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, cp, authProperties);

            return new AuthenticatedUser(token, user.Id, user.Email, user.FirstName, user.LastName, user.IsAdmin);
        }
    }

    [RequireAnonymousRole]
    public class AuthenticatedUserQuery : IQuery<AuthenticatedUser>
    {
        public AuthenticatedUserQuery(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; }
        public string Password { get; }

    }

    public class AuthenticatedUser
    {
        public AuthenticatedUser(string token, long id, string email, string firstName, string lastName, bool isAdmin)
        {
            Token = token;
            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            IsAdmin = isAdmin;
        }

        public string Token { get; }
        public long Id { get; }
        public string Name { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public bool IsAdmin { get; }
    }


}
