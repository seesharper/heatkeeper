namespace HeatKeeper.Server.Users
{
    public interface IPasswordPolicy
    {
        void Apply(string password, string confirmedPassword);
    }
}