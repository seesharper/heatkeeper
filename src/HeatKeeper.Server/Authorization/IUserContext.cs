namespace HeatKeeper.Server.Authorization
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
