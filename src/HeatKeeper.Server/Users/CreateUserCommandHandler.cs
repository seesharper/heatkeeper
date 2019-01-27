using DbReader;
using HeatKeeper.Server.CQRS;
using HeatKeeper.Server.Database;
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Users
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public CreateUserCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            await ((DbCommand)dbConnection.CreateCommand(sqlProvider.InsertUser, command)).ExecuteNonQueryAsync();
        }
    }
}