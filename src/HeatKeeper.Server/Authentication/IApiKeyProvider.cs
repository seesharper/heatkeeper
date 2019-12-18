using System;
using System.Collections.Generic;
using System.Security.Claims;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Authentication
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
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, userContext.Email),
                new Claim(ClaimTypes.GivenName, userContext.FirstName),
                new Claim(ClaimTypes.Surname, userContext.LastName),
                new Claim(ClaimTypes.Role, "reporter"),
                new Claim(ClaimTypes.Sid, userContext.Id.ToString())
            };

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