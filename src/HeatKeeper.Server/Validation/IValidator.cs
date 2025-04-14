namespace HeatKeeper.Server.Validation;

public interface IValidator<in T>
{
    Task Validate(T value);
}

