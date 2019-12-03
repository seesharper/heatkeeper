using System;

namespace HeatKeeper.Server.Authorization
{
    public class AuthorizationFailedException : Exception
    {
        public AuthorizationFailedException(string message) : base(message)
        {
        }
    }
}
