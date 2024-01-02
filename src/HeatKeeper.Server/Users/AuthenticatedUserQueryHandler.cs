using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authentication;
using HeatKeeper.Server.Authorization;
using Microsoft.AspNetCore.Http;

namespace HeatKeeper.Server.Users
{
    public class AuthenticatedUserQueryHandler : IQueryHandler<AuthenticatedUserQuery, AuthenticatedUser>
    {
        private readonly IPasswordManager _passwordManager;
        private readonly ITokenProvider _tokenProvider;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IRefreshTokenProvider _refreshTokenProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticatedUserQueryHandler(IPasswordManager passwordManager, ITokenProvider tokenProvider, IQueryExecutor queryExecutor, IRefreshTokenProvider refreshTokenProvider, IHttpContextAccessor httpContextAccessor)
        {
            this._passwordManager = passwordManager;
            this._tokenProvider = tokenProvider;
            this._queryExecutor = queryExecutor;
            _refreshTokenProvider = refreshTokenProvider;
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

            var token = _tokenProvider.CreateToken(claims, DateTime.UtcNow.AddDays(7));

            var refreshToken = _refreshTokenProvider.CreateRefreshToken();

            _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", refreshToken.Token, new CookieOptions
            {
                Expires = refreshToken.Expires,
                HttpOnly = true,
                Secure = true
            });

            // var cookies = _httpContextAccessor.HttpContext.Response.Cookies.

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
