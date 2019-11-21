namespace HeatKeeper.Server.Host.Users
{
    public class AuthenticateUserResponse
    {
        public AuthenticateUserResponse(string token, long id, string name, string email, bool isAdmin)
        {
            Token = token;
            Id = id;
            Name = name;
            Email = email;
            IsAdmin = isAdmin;
        }

        public string Token { get; }
        public long Id { get; }
        public string Name { get; }
        public string Email { get; }

        public bool IsAdmin { get; }
    }
}
