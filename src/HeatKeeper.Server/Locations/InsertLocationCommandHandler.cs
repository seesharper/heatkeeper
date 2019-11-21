using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;

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
}
