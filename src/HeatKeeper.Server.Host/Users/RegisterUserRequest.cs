namespace HeatKeeper.Server.Host.Users
{
    public class RegisterUserRequest
    {
        public RegisterUserRequest(string name, string email, bool isAdmin, string password, string confirmedPassword)
        {
            Name = name;
            Email = email;
            IsAdmin = isAdmin;
            Password = password;
            ConfirmedPassword = confirmedPassword;
        }

        public string Name { get; }
        public string Email { get; }
        public bool IsAdmin { get; }
        public string Password { get; }
        public string ConfirmedPassword { get; }
    }
}