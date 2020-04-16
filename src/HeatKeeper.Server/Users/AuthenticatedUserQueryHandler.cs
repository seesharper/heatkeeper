using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authentication;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Users
{
    public class AuthenticatedUserQueryHandler : IQueryHandler<AuthenticatedUserQuery, AuthenticatedUser>
    {
        private readonly IPasswordManager passwordManager;
        private readonly ITokenProvider tokenProvider;
        private readonly IQueryExecutor queryExecutor;

        public AuthenticatedUserQueryHandler(IPasswordManager passwordManager, ITokenProvider tokenProvider, IQueryExecutor queryExecutor)
        {
            this.passwordManager = passwordManager;
            this.tokenProvider = tokenProvider;
            this.queryExecutor = queryExecutor;
        }

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

            var token = tokenProvider.CreateToken(claims, DateTime.UtcNow.AddDays(7));

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
