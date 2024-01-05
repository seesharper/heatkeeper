using System;
using System.Security.Cryptography;

namespace HeatKeeper.Server.Authentication.RefreshTokens;


public interface IRefreshTokenProvider
{
    RefreshToken CreateRefreshToken();
}


public class RefreshTokenProvider : IRefreshTokenProvider
{
    public RefreshToken CreateRefreshToken()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expires = DateTime.UtcNow.AddDays(7);
        return new RefreshToken(token, expires);
    }
}

public record RefreshToken(string Token, DateTime Expires);
