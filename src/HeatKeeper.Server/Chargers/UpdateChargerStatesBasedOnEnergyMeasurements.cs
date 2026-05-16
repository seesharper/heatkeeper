using HeatKeeper.Server.Measurements;

namespace HeatKeeper.Server.Chargers;

[RequireReporterRole]
public record UpdateChargerStatesBasedOnEnergyMeasurementsCommand(MeasurementCommand[] Measurements);

public record ChargerEnergyInfo(long ChargerId, double EnergyThreshold, ChargerState ChargerState);

public class UpdateChargerStatesBasedOnEnergyMeasurements(IDbConnection dbConnection, ISqlProvider sqlProvider, ICommandExecutor commandExecutor)
    : ICommandHandler<UpdateChargerStatesBasedOnEnergyMeasurementsCommand>
{
    public async Task HandleAsync(UpdateChargerStatesBasedOnEnergyMeasurementsCommand command, CancellationToken cancellationToken = default)
    {
        var latestPerSensor = command.Measurements
            .GroupBy(m => m.SensorId)
            .Select(g => g.OrderByDescending(m => m.Created).First());

        foreach (var measurement in latestPerSensor)
        {
            var charger = (await dbConnection.ReadAsync<ChargerEnergyInfo>(
                sqlProvider.GetChargerByEnergySensorExternalId,
                new { SensorExternalId = measurement.SensorId })).SingleOrDefault();

            if (charger is null) continue;

            var newState = measurement.Value > charger.EnergyThreshold ? ChargerState.Active : ChargerState.Idle;
            if (newState != charger.ChargerState)
                await commandExecutor.ExecuteAsync(new SetChargerStateCommand(charger.ChargerId, newState), cancellationToken);
        }
    }
}
