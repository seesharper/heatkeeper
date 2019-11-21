using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Sensors
{
    public class CreateSensorCommandHander : ICommandHandler<CreateSensorCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public CreateSensorCommandHander(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(CreateSensorCommand command, CancellationToken cancellationToken = default)
            => await ((DbCommand)dbConnection.CreateCommand(sqlProvider.InsertSensor, command)).ExecuteNonQueryAsync();
    }
}
