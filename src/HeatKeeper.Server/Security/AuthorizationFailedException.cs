using System;

namespace HeatKeeper.Server.Security
{
    public class AuthorizationFailedException : Exception
    {
        public AuthorizationFailedException(string message) : base(message)
        {
        }
    }
}
