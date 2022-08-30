using System.Threading.Tasks;

namespace ClientApp.Services
{
    public interface ITokenService
    {
        Task<string> GetTokenAsync();
    }
}
