using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using System;
using CQRS.Query.Abstractions;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Security;

namespace HeatKeeper.Server.Users
{
    public class AuthenticatedUserQueryHandler : IQueryHandler<AuthenticatedUserQuery, AuthenticatedUserQueryResult>
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

        public async Task<AuthenticatedUserQueryResult> HandleAsync(AuthenticatedUserQuery query, CancellationToken cancellationToken = default)
        {
            var user = await queryExecutor.ExecuteAsync(new GetUserQuery(query.UserName));

            if (user == null || !passwordManager.VerifyPassword(query.Password, user.HashedPassword))
            {
                throw new AuthenticationFailedException("Invalid username/password");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "admin" : "user"),
                new Claim(ClaimTypes.Sid, user.Id.ToString())
            };

            var token = tokenProvider.CreateToken(claims, DateTime.UtcNow.AddDays(7));

            return new AuthenticatedUserQueryResult(token, user.Id, user.Name, user.Email, user.IsAdmin);
        }
    }

    [RequireNoRole]
    public class AuthenticatedUserQuery : IQuery<AuthenticatedUserQueryResult>
    {
        public AuthenticatedUserQuery(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public string UserName { get; }
        public string Password { get; }

    }

    public class AuthenticatedUserQueryResult
    {
        public AuthenticatedUserQueryResult(string token, long id, string name, string email, bool isAdmin)
        {
            Token = token;
            Id = id;
            Name = name;
            Email = email;
            IsAdmin = isAdmin;
        }

        public string Token { get; }
        public long Id { get; }
        public string Name { get; }
        public string Email { get; }

        public bool IsAdmin { get; }
    }


}
