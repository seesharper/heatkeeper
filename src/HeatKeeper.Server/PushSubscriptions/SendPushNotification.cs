using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Abstractions.Configuration;
using HeatKeeper.Server.Authorization;
using Microsoft.Extensions.Configuration;
using WebPush;

namespace HeatKeeper.Server.PushSubscriptions;

[RequireBackgroundRole]
public record SendPushNotificationCommand(PushSubscription Subscription, string payLoad);

public class SendPushNotificationCommandHandler(IWebPushClient webPushClient, IConfiguration configuration) : ICommandHandler<SendPushNotificationCommand>
{
    public async Task HandleAsync(SendPushNotificationCommand command, CancellationToken cancellationToken = default)
    {
        var vapidDetails = new VapidDetails(
            subject: configuration.GetVapidSubject(),
            publicKey: configuration.GetVapidPublicKey(),
            privateKey: configuration.GetVapidPrivateKey()
        );
        try
        {
            await webPushClient.SendNotificationAsync(command.Subscription, command.payLoad, vapidDetails, cancellationToken: cancellationToken);
        }
        catch (System.Exception)
        {
            //throw;
        }

    }
}