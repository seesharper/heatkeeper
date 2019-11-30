using Newtonsoft.Json;

namespace HeatKeeper.Server.Users
{
    public abstract class UserCommand
    {
        [JsonIgnore]
        public long UserId { get; set; }
        public string Email { set; get; }
        public string FirstName { set; get; }
        public string LastName { set; get; }
    }
}
