namespace HeatKeeper.Server.Host.Users
{
    public class RegisterUserRequest
    {
        public RegisterUserRequest(string email, string firstName, string lastName, bool isAdmin, string password, string confirmedPassword)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            IsAdmin = isAdmin;
            Password = password;
            ConfirmedPassword = confirmedPassword;
        }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public bool IsAdmin { get; }
        public string Password { get; }
        public string ConfirmedPassword { get; }
    }
}
