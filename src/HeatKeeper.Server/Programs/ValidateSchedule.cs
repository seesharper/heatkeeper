using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using Cronos;
using HeatKeeper.Server.Validation;

namespace HeatKeeper.Server.Programs;

public class ValidateSchedule<TCommand>(ICommandHandler<TCommand> handler) : ICommandHandler<TCommand> where TCommand : IScheduleCommand
{
    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        if (IsValidCronExpression(command.CronExpression))
        {
            await handler.HandleAsync(command, cancellationToken);
        }
        else
        {
            throw new ValidationFailedException($"The cron expression {command.CronExpression} is not valid for schedule {command.Name}");
        }
    }

    private static bool IsValidCronExpression(string expression)
    {
        try
        {
            CronExpression.Parse(expression);
            return true;
        }
        catch (CronFormatException)
        {
            return false;
        }
    }



}