using System.Threading.Tasks;
using HeatKeeper.Server.Programs.Api;
using HeatKeeper.Server.Schedules.Api;

namespace HeatKeeper.Server.Events;

public interface IEventHandler<TEvent>
{
    Task HandleEvent(TEvent @event);
}

public interface IEventPublisher
{
    Task PublishEvent<TEvent>(TEvent @event);
}

public interface IBeforeCommandIsExecuted<TCommand>
{
    Task BeforeCommandIsExecuted(TCommand command);
}

public interface IAfterCommandIsExecuted
{
    Task AfterCommandIsExecuted<TCommand>(TCommand command);
}

public class BeforeScheduleIsDeleted : IBeforeCommandIsExecuted<CreateScheduleCommand>
{
    public Task BeforeCommandIsExecuted(CreateScheduleCommand command) => throw new NotImplementedException();
}
