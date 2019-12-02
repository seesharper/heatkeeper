using System.Threading.Tasks;
using System.Threading;
using System.Data.Common;
using System.Data;
using HeatKeeper.Server.Database;
using DbReader;
using CQRS.Command.Abstractions;
using System;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Measurements
{
    public class CreateMeasurementsCommandHandler : ICommandHandler<CreateMeasurementCommand[]>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public CreateMeasurementsCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(CreateMeasurementCommand[] commands, CancellationToken cancellationToken = default)
        {
            foreach (var command in commands)
            {
                await ((DbCommand)dbConnection.CreateCommand(sqlProvider.InsertMeasurement, command)).ExecuteNonQueryAsync();
            }
        }
    }

    [RequireReporterRole]
    public class CreateMeasurementCommand
    {
        public CreateMeasurementCommand(string sensorId, MeasurementType measurementType, double value)
        {
            SensorId = sensorId;
            MeasurementType = measurementType;
            Value = value;
        }

        public string SensorId { get; }
        public MeasurementType MeasurementType { get; }
        public double Value { get; }
    }
}
