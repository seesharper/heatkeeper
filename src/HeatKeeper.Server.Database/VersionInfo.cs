using System;

namespace HeatKeeper.Server.Database
{
    public class VersionInfo
    {
        public long Version { get; set; }

        public DateTime AppliedOn { get; set; }

        public string Description { get; set; }
    }
}
