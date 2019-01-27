namespace HeatKeeper.Server.Host.Users
{
    public class AuthenticateUserRequest
    {
        public AuthenticateUserRequest(string name, string password)
        {
            Name = name;
            Password = password;
        }

        public string Name { get; }
        public string Password { get; }
    }
}