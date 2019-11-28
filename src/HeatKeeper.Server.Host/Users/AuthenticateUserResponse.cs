namespace HeatKeeper.Server.Host.Users
{
    public class AuthenticateUserResponse
    {
        public AuthenticateUserResponse(string token, long id, string email, string firstName, string lastName, bool isAdmin)
        {
            Token = token;
            Id = id;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            IsAdmin = isAdmin;
        }

        public string Token { get; }
        public long Id { get; }

        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public bool IsAdmin { get; }
    }
}
