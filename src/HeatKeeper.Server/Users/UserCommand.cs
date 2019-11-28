namespace HeatKeeper.Server.Users
{
    public abstract class UserCommand
    {
        public UserCommand(string email, string firstName, string lastName, bool isAdmin)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            IsAdmin = isAdmin;
        }

        public long Id { get; internal set; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public bool IsAdmin { get; }
    }
}
