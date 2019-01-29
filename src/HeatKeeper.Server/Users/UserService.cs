using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HeatKeeper.Server.CQRS;
using HeatKeeper.Server.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HeatKeeper.Server.Users
{
    public class UserService : IUserService
    {
        private readonly ICommandExecutor commandExecutor;
        private readonly IQueryExecutor queryExecutor;

        private Settings settings;

        private readonly PasswordHasher<UserService> passwordHasher = new PasswordHasher<UserService>();

        public UserService(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor, IOptions<Settings> settings)
        {
            this.settings = settings.Value;
            this.commandExecutor = commandExecutor;
            this.queryExecutor = queryExecutor;
        }

        public async Task CreateUser(NewUser user)
        {
            var hashedPassword = passwordHasher.HashPassword(this, user.Password);
            var createUserCommand = new CreateUserCommand(user.Name, user.Email, hashedPassword, user.IsAdmin);
            await commandExecutor.ExecuteAsync(createUserCommand);
        }

        public async Task<string> Authenticate(string name, string password)
        {
            var user = await queryExecutor.ExecuteAsync(new GetUserQuery(name));
            if (user == null && name == AdminUser.UserName)
            {
                await CreateUser(new NewUser(AdminUser.UserName, "E-mail address goes here", true, AdminUser.DefaultPassword));
                return await Authenticate(name, password);
            }
            if (user != null)
            {
                var result = passwordHasher.VerifyHashedPassword(this, user.HashedPassword, password);
                if (result == PasswordVerificationResult.Success)
                {
                    return CreateToken(user);
                }
            }
            return null;
        }

        private string CreateToken(GetUserQueryResult user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(settings.Secret);
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.Role, user.IsAdmin ? "admin" : "user"));
            claims.Add(new Claim(ClaimTypes.Sid, user.Id.ToString()));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}