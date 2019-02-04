using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using HeatKeeper.Abstractions.CQRS;

namespace HeatKeeper.Server.Users
{
    public interface IAuthenticationManager
    {
        Task<AuthenticationResult> Authenticate(string userName, string password);
    }

    public class AuthenticationManager : IAuthenticationManager
    {
        private readonly IQueryExecutor queryExecutor;
        private readonly ICommandExecutor commandExecutor;
        private readonly IPasswordManager passwordManager;
        private readonly ITokenProvider tokenProvider;

        public AuthenticationManager(
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor,
            IPasswordManager passwordManager,
            ITokenProvider tokenProvider)
        {
            this.queryExecutor = queryExecutor;
            this.commandExecutor = commandExecutor;
            this.passwordManager = passwordManager;
            this.tokenProvider = tokenProvider;
        }

        public async Task<AuthenticationResult> Authenticate(string userName, string password)
        {
            var user = await queryExecutor.ExecuteAsync(new GetUserQuery(userName));
            if (user == null && userName == AdminUser.UserName)
            {
                await commandExecutor.ExecuteAsync(new RegisterUserCommand(AdminUser.UserName, "Email", AdminUser.DefaultPassword, true));
                return await Authenticate(userName, password);
            }

            if (user == null || !passwordManager.VerifyPassword(password, user.HashedPassword))
            {
                return new AuthenticationResult(string.Empty, isAuthenticated: false);
            }

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.Role, user.IsAdmin ? "admin" : "user"));
            claims.Add(new Claim(ClaimTypes.Sid, user.Id.ToString()));

            return new AuthenticationResult(tokenProvider.CreateToken(claims, DateTime.UtcNow.AddDays(7)), isAuthenticated: true);
        }
    }


    public class AuthenticationResult
    {
        public AuthenticationResult(string token, bool isAuthenticated)
        {
            Token = token;
            IsAuthenticated = isAuthenticated;
        }

        public string Token { get; }
        public bool IsAuthenticated { get; }
    }
}