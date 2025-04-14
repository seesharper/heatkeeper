using HeatKeeper.Server.Programs.Api;
using Microsoft.AspNetCore.Http.HttpResults;

namespace HeatKeeper.Server.Schedules;

public class WhenScheduleIsCreated(ICommandExecutor commandExecutor, ICommandHandler<CreateScheduleCommand> handler) : ICommandHandler<CreateScheduleCommand>
{
    public async Task HandleAsync(CreateScheduleCommand command, CancellationToken cancellationToken = default)
    {
        await handler.HandleAsync(command, cancellationToken);
        var result = command.GetResult().Result;
        if (result is Created<ResourceId> created)
        {
            await commandExecutor.ExecuteAsync(new AddScheduleToJanitorCommand(created.Value.Id, command.Name, command.CronExpression), cancellationToken);
        }
    }
}
