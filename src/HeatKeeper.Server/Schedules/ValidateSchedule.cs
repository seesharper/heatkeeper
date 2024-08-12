
using Cronos;

namespace HeatKeeper.Server.Schedules;

public class ValidateSchedule : IValidator<IScheduleCommand>
{
    public Task Validate(IScheduleCommand command)
    {
        if (!IsValidCronExpression(command.CronExpression))
        {
            command.SetProblemResult($"The cron expression {command.CronExpression} is not valid for schedule {command.Name}", StatusCodes.Status400BadRequest);
        }
        
        return Task.CompletedTask;        
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