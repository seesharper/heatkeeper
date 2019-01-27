using System.Threading.Tasks;

namespace HeatKeeper.Server.Users
{
    public interface IUserService
    {
        Task CreateUser(string userName, string passWord, bool isAdmin, string email);

        Task<string> Authenticate(string name, string password);
    }
}