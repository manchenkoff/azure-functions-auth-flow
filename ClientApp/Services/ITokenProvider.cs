using System.Threading.Tasks;

namespace ClientApp.Services
{
    public interface ITokenProvider
    {
        public Task<string> GetToken();

        public Task<string> GetTokenObo(string authToken);
    }
}
