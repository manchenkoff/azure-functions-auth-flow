using ClientApp.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ClientApp.Services
{
    public interface IRemoteUser
    {
        public Task<UserModel> Current(ILogger logger);

        public Task<UserModel> CurrentOnBehalfOf(string userToken, ILogger logger);
    }
}
