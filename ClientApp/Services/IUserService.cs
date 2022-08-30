using System.Threading.Tasks;

namespace ClientApp.Services
{
    public interface IUserService
    {
        Task<string> GetRemoteUser();
    }
}
