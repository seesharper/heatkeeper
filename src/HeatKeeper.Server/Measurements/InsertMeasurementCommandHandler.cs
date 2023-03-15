using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Measurements
{
    public class InsertMeasurementCommandHandler : ICommandHandler<MeasurementCommand>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ISqlProvider _sqlProvider;

        public InsertMeasurementCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            _dbConnection = dbConnection;
            _sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(MeasurementCommand command, CancellationToken cancellationToken = default)
            => await _dbConnection.ExecuteAsync(_sqlProvider.InsertMeasurement, command);
    }
}
