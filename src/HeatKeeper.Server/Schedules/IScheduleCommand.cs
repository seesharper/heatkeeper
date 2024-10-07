namespace HeatKeeper.Server.Schedules;


public interface IScheduleCommand : IProblemCommand
{
    string Name { get; }
    string CronExpression { get; }
}
