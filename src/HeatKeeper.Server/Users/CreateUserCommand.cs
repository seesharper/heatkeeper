namespace HeatKeeper.Server.Users
{
    public class CreateUserCommand : UserCommand
    {
        public CreateUserCommand(string name, string email, bool isAdmin, string hashedPassword) : base(name, email, isAdmin)
        {
            HashedPassword = hashedPassword;
        }

        public string HashedPassword { get; }
    }
}