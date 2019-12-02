namespace HeatKeeper.Server.Authentication
{
    public interface IPasswordManager
    {
        string GetHashedPassword(string password);

        bool VerifyPassword(string password, string hashedPassword);
    }
}
