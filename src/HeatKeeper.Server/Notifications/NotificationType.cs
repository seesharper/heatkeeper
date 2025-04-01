namespace HeatKeeper.Server.Notifications;

public enum NotificationType
{
    DeadSensors = 1,
    ProgramChanged = 2,
    EnergyPrice = 3,
    Measurement = 4
}

public enum NotificationDeliveryType
{
    Immediate = 1,
    Scheduled = 2
}

public enum OperatorType
{
    Equal = 1,
    NotEqual = 2,
    GreaterThan = 3,
    GreaterThanOrEqual = 4,
    LessThan = 5,
    LessThanOrEqual = 6
}

public enum ConditionType
{
    EnergyPrice = 1,
    MeasurementValue = 2,
    MeasurementType = 3,
    Zone = 4,
}

public record Condition(ConditionType Type, bool Required);


public static class AllowedOperatorTypes
{
    public static readonly Dictionary<Condition, OperatorType[]> AllowedOperators = new()
    {
        { new Condition(ConditionType.EnergyPrice, true), new[] { OperatorType.Equal, OperatorType.NotEqual, OperatorType.GreaterThan, OperatorType.GreaterThanOrEqual, OperatorType.LessThan, OperatorType.LessThanOrEqual } },
        { new Condition(ConditionType.MeasurementValue, true), new[] { OperatorType.Equal, OperatorType.NotEqual, OperatorType.GreaterThan, OperatorType.GreaterThanOrEqual, OperatorType.LessThan, OperatorType.LessThanOrEqual } },
        { new Condition(ConditionType.MeasurementType, true), new[] { OperatorType.Equal, OperatorType.NotEqual } },
        { new Condition(ConditionType.Zone, true), new[] { OperatorType.Equal, OperatorType.NotEqual } }
    };
}