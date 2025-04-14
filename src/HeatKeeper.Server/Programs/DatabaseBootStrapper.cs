using System.Threading.Tasks;
using HeatKeeper.Abstractions;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Programs;

[Order(1)]
public class DatabaseBootStrapper(IDatabaseMigrator databaseMigrator) : IBootStrapper
{
    public async Task Execute() => databaseMigrator.Migrate();
}