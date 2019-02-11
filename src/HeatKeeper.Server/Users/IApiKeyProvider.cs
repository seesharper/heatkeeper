using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace HeatKeeper.Server.Users
{
    public interface IApiKeyProvider
    {
        ApiKey CreateApiKey();
    }

    public class ApiKeyProvider : IApiKeyProvider
    {
        private readonly ITokenProvider tokenProvider;
        private readonly IUserContext userContext;

        public ApiKeyProvider(ITokenProvider tokenProvider, IUserContext userContext)
        {
            this.tokenProvider = tokenProvider;
            this.userContext = userContext;
        }

        public ApiKey CreateApiKey()
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, userContext.Name));
            claims.Add(new Claim(ClaimTypes.Email, userContext.Email));
            claims.Add(new Claim(ClaimTypes.Role, "reporter"));
            claims.Add(new Claim(ClaimTypes.Sid, userContext.Id.ToString()));

            var token = tokenProvider.CreateToken(claims, DateTime.MaxValue);

            return new ApiKey(token);
        }
    }


    public class ApiKey
    {
        public ApiKey(string token)
        {
            Token = token;
        }

        public string Token { get; }
    }

}