using System;

namespace HeatKeeper.Server.Database
{
    public record Sql(string Value)
    {
        public static implicit operator string(Sql sql) => sql.Value;
        public static implicit operator Sql(string value) => new(value);
    }
}