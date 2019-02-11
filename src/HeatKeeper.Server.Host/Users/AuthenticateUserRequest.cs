namespace HeatKeeper.Server.Host.Users
{
    public class AuthenticateUserRequest
    {
        public AuthenticateUserRequest(string userName, string password)
        {
            Username = userName;
            Password = password;
        }

        public string Username { get; }
        public string Password { get; }
    }
}