using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Measurements;

namespace HeatKeeper.Server.Programs;


public record CreateHeatStatusMeasurementsCommand();

public class CreateHeatStatusMeasurementsCommandHandler : ICommandHandler<CreateHeatStatusMeasurementsCommand>
{
    public CreateHeatStatusMeasurementsCommandHandler(IQueryExecutor queryExecutor)
    {

        //Measurement 
    }

    public Task HandleAsync(CreateHeatStatusMeasurementsCommand command, CancellationToken cancellationToken = default)
    {
        //Get all Mqtt topics for all zones in all locations

        return Task.CompletedTask;
    }


}