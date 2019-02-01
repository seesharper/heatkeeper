using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace HeatKeeper.Server.Users
{
    public interface ITokenProvider
    {
        string CreateToken(IEnumerable<Claim> claims, DateTime expires);
    }
}