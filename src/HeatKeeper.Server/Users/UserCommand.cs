namespace HeatKeeper.Server.Users
{
    public abstract class UserCommand
    {
        public UserCommand(string name, string email, bool isAdmin)
        {
            Name = name;
            Email = email;
            IsAdmin = isAdmin;
        }

        public long Id { get; internal set; }
        public string Name { get; }
        public string Email { get; }
        public bool IsAdmin { get; }
    }
}