using System;

namespace HeatKeeper.Server.Database.Migrations
{
    public class AppliesToVersionAttribute : Attribute
    {
        public AppliesToVersionAttribute(int version)
        {
            Version = version;
        }

        public int Version { get; }

        public int Order { get; set; }
    }
}
