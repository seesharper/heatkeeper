namespace HeatKeeper.Server.Authentication
{
    public interface IPasswordPolicy
    {
        void Apply(string password, string confirmedPassword);
    }
}
