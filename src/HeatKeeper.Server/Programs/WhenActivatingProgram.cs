using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Locations;
using HeatKeeper.Server.PushSubscriptions;
using WebPush;

namespace HeatKeeper.Server.Programs;

public class WhenActivatingProgram(ICommandHandler<ActivateProgramCommand> handler, IQueryExecutor queryExecutor, ICommandExecutor commandExecutor) : ICommandHandler<ActivateProgramCommand>
{
    public async Task HandleAsync(ActivateProgramCommand command, CancellationToken cancellationToken = default)
    {
        await handler.HandleAsync(command, cancellationToken);

        // Send a notification to all push subscribers that the program has been activated        
        var programDetails = await queryExecutor.ExecuteAsync(new GetProgramDetailsQuery(command.ProgramId), cancellationToken);
        var locationDetails = await queryExecutor.ExecuteAsync(new GetLocationDetailsQuery(programDetails.LocationId), cancellationToken);
        var subscriptionsQueryResults = await queryExecutor.ExecuteAsync(new GetPushSubscriptionsByLocationQuery(programDetails.LocationId), cancellationToken);

        foreach (var subscription in subscriptionsQueryResults)
        {
            string payLoad = "The program " + programDetails.Name + " has been activated at " + locationDetails.Name + ".";
            var pushSubscription = new PushSubscription(subscription.Endpoint, subscription.P256dh, subscription.Auth);
            await commandExecutor.ExecuteAsync(new SendPushNotificationCommand(pushSubscription, payLoad), cancellationToken);
        }

    }
}