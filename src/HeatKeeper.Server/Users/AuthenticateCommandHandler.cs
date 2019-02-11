using DbReader;
using HeatKeeper.Abstractions.CQRS;
using HeatKeeper.Server.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Users
{
    public class AuthenticateCommandHandler : ICommandHandler<AuthenticateCommand>
    {
        private readonly IPasswordManager passwordManager;
        private readonly ITokenProvider tokenProvider;
        private readonly IQueryExecutor queryExecutor;
        private readonly ICommandExecutor commandExecutor;

        public AuthenticateCommandHandler(IPasswordManager passwordManager, ITokenProvider tokenProvider, IQueryExecutor queryExecutor, ICommandExecutor commandExecutor)
        {
            this.passwordManager = passwordManager;
            this.tokenProvider = tokenProvider;
            this.queryExecutor = queryExecutor;
            this.commandExecutor = commandExecutor;
        }

        public async Task HandleAsync(AuthenticateCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var user = await queryExecutor.ExecuteAsync(new GetUserQuery(command.UserName));
            if (user == null && command.UserName == AdminUser.UserName)
            {
                await commandExecutor.ExecuteAsync(new RegisterUserCommand(AdminUser.UserName, "Email", AdminUser.DefaultPassword, true));
                await HandleAsync(command);
                return;
            }

            if (user == null || !passwordManager.VerifyPassword(command.Password, user.HashedPassword))
            {
                throw new AuthenticationFailedException();
            }


            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.Role, user.IsAdmin ? "admin" : "user"));
            claims.Add(new Claim(ClaimTypes.Sid, user.Id.ToString()));

            command.Token = tokenProvider.CreateToken(claims, DateTime.UtcNow.AddDays(7));
        }
    }

    public class AuthenticateCommand
    {
        public AuthenticateCommand(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public string UserName { get; }
        public string Password { get; }

        public string Token {get;set;}
    }

    public class AuthenticationFailedException : Exception
    {

    }
}