namespace HeatKeeper.Server.Host.Users
{
    public class CreateUserRequest
    {
        public CreateUserRequest(string name, string email, bool isAdmin, string password)
        {
            Name = name;
            Email = email;
            IsAdmin = isAdmin;
            Password = password;
        }

        public string Name { get; }
        public string Email { get; }
        public bool IsAdmin { get; }
        public string Password { get; }
    }
}