using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using DbReader;
using HeatKeeper.Server.CQRS;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Locations
{
    public class CreateLocationCommandHandler : ICommandHandler<CreateLocationCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public CreateLocationCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(CreateLocationCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            await ((DbCommand)dbConnection.CreateCommand(sqlProvider.InsertLocation, command)).ExecuteNonQueryAsync();
        }
    }
}