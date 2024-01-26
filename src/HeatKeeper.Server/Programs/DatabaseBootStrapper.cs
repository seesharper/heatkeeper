using System.Threading.Tasks;
using HeatKeeper.Abstractions;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Programs;

[Order(2)]
public class DatabaseBootStrapper : IBootStrapper
{
    private readonly IDatabaseMigrator _databaseMigrator;

    public DatabaseBootStrapper(IDatabaseMigrator databaseMigrator)
    {
        _databaseMigrator = databaseMigrator;
    }

    public async Task Execute()
    {
        _databaseMigrator.Migrate();
    }
}