namespace HeatKeeper.Server.Host.Users
{
    public class AuthenticateUserRequest
    {
        public AuthenticateUserRequest(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; }
        public string Password { get; }
    }
}
