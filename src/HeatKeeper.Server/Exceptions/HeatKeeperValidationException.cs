using System;

namespace HeatKeeper.Server.Exceptions
{
    public class HeatKeeperValidationException : Exception
    {
        public HeatKeeperValidationException(string message) : base(message)
        {
        }
    }
}