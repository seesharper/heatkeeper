using System.Text;
using System.Threading.Channels;
using WebPush;

namespace HeatKeeper.Server.Notifications;

public class NotificationsQueue
{
    private readonly Channel<Notification> _channel = Channel.CreateUnbounded<Notification>();

    public async Task EnqueueAsync(Notification notification)
    {
        await _channel.Writer.WriteAsync(notification);
    }

    public async IAsyncEnumerable<Notification> DequeueAll()
    {
        await foreach (var item in _channel.Reader.ReadAllAsync())
        {
            yield return item;
        }
    }
}


public record Notification(PushSubscription PushSubscription, string PayLoad);