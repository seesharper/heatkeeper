using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Abstractions.CQRS;
using HeatKeeper.Server.Database;
using DbReader;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System;

namespace HeatKeeper.Server.Users
{
    public class AuthenticatedUserQueryHandler : IQueryHandler<AuthenticatedUserQuery,AuthenticatedUserQueryResult>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;
        private readonly IPasswordManager passwordManager;
        private readonly ITokenProvider tokenProvider;
        private readonly IQueryExecutor queryExecutor;
        private readonly ICommandExecutor commandExecutor;

        public AuthenticatedUserQueryHandler(IPasswordManager passwordManager, ITokenProvider tokenProvider, IQueryExecutor queryExecutor, ICommandExecutor commandExecutor)
        {
            this.passwordManager = passwordManager;
            this.tokenProvider = tokenProvider;
            this.queryExecutor = queryExecutor;
            this.commandExecutor = commandExecutor;
        }

        public async Task<AuthenticatedUserQueryResult> HandleAsync(AuthenticatedUserQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            var user = await queryExecutor.ExecuteAsync(new GetUserQuery(query.UserName));
            if (user == null && query.UserName == AdminUser.UserName)
            {
                await commandExecutor.ExecuteAsync(new RegisterUserCommand(AdminUser.UserName, "admin@no.org",true, AdminUser.DefaultPassword,AdminUser.DefaultPassword));
                return await HandleAsync(query);
            }

            if (user == null || !passwordManager.VerifyPassword(query.Password, user.HashedPassword))
            {
                throw new AuthenticationFailedException("Invalid username/password");
            }


            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.Role, user.IsAdmin ? "admin" : "user"));
            claims.Add(new Claim(ClaimTypes.Sid, user.Id.ToString()));

            var token = tokenProvider.CreateToken(claims, DateTime.UtcNow.AddDays(7));

            return new AuthenticatedUserQueryResult(token, user.Id, user.Name, user.Email, user.IsAdmin );
        }
    }
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