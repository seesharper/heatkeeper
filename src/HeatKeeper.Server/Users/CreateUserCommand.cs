namespace HeatKeeper.Server.Users
{
    public class CreateUserCommand
    {
        public CreateUserCommand(string name, string email, string hashedPassword, bool isAdmin)
        {
            Name = name;
            Email = email;
            HashedPassword = hashedPassword;
            IsAdmin = isAdmin;
        }

        public string Name { get; }
        public string Email { get; }
        public string HashedPassword { get; }
        public bool IsAdmin { get; }
    }
}