using System;

namespace HeatKeeper.Server.Security
{
    public class HeatKeeperSecurityException : Exception
    {
        public HeatKeeperSecurityException(string message) : base(message)
        {
        }
    }
}