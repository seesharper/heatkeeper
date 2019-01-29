namespace HeatKeeper.Server.Users
{
    public class NewUser
    {
        public NewUser(string name, string email, bool isAdmin, string password)
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