using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Locations
{
    public class InsertLocationCommandHandler : ICommandHandler<InsertLocationCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public InsertLocationCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(InsertLocationCommand command, CancellationToken cancellationToken = default)
        {
            await dbConnection.ExecuteAsync(sqlProvider.InsertLocation, command);
            command.Id = await dbConnection.ExecuteScalarAsync<long>(sqlProvider.GetLocationId, new { command.Name });
        }
    }

    [RequireAdminRole]
    public class InsertLocationCommand
    {
        public InsertLocationCommand(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }

        public string Description { get; }

        public long Id { get; set; }
    }
}
