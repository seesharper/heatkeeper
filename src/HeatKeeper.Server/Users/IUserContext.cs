namespace HeatKeeper.Server.Users
{
    public interface IUserContext
    {
        long Id { get; }

        string Name { get; }

        string Email { get; }

        bool IsAdmin { get; }

        string Role { get; }
    }
}
