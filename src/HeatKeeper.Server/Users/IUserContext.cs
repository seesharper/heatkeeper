namespace HeatKeeper.Server.Users
{
    public interface IUserContext
    {
        long Id { get; }

        string FirstName { get; }

        string LastName { get; }

        string Email { get; }

        bool IsAdmin { get; }

        string Role { get; }
    }
}
