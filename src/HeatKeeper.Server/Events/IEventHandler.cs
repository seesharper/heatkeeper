using System.Threading.Tasks;
using HeatKeeper.Server.Export;

namespace HeatKeeper.Server.Events;


public interface IEventHandler<TEvent>
{
    Task HandleEvent(TEvent @event);
}

public interface IEventPublisher
{
    Task PublishEvent<TEvent>(TEvent @event);
}

