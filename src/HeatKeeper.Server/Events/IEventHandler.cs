using System.Threading.Tasks;

namespace HeatKeeper.Server.Events;

public interface IEventHandler<TEvent>
{
    Task HandleEvent(TEvent @event);
}

public interface IEventPublisher
{
    Task PublishEvent<TEvent>(TEvent @event);
}

