namespace HeatKeeper.Server.Host.Users
{
    public class GetUserResponse
    {
        public GetUserResponse(long id, string userName, string email, bool isAdmin)
        {
            Id = id;
            UserName = userName;
            Email = email;
            IsAdmin = isAdmin;
        }

        public long Id { get; }
        public string UserName { get; }
        public string Email { get; }
        public bool IsAdmin { get; }
    }
}