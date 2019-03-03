using System;

namespace HeatKeeper.Server.Exceptions
{
    public class HeatKeeperConflictException : Exception
    {
        public HeatKeeperConflictException(string message) : base(message)
        {
        }
    }
}