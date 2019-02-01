namespace HeatKeeper.Server.Users
{
    public interface IPasswordManager
    {
        string GetHashedPassword(string password);

        bool VerifyPassword(string password, string hashedPassword);
    }
}