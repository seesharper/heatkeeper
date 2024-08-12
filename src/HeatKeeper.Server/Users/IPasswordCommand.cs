namespace HeatKeeper.Server.Users;

public interface IPasswordCommand : IProblemCommand
{
    string NewPassword { get; }
    string ConfirmedPassword { get; }
}