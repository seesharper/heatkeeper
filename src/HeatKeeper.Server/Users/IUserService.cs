using System.Threading.Tasks;

namespace HeatKeeper.Server.Users
{
    public interface IUserService
    {
        Task CreateUser(NewUser user);

        Task<string> Authenticate(string name, string password);
    }
}