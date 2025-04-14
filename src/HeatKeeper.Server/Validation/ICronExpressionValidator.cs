using Cronos;

namespace HeatKeeper.Server.Validation;

public interface ICronExpressionValidator
{
    bool Validate(string cronExpression);
}

public class CronExpressionValidator : ICronExpressionValidator
{
    public bool Validate(string cronExpression)
    {
       try
        {
            CronExpression.Parse(cronExpression);
            return true;
        }
        catch (CronFormatException)
        {
            return false;
        }
    }
}