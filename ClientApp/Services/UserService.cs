using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientApp.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenService;
        private readonly string _serviceUrl;

        public UserService(IHttpClientFactory httpClientFactory, ITokenService tokenService, string serviceUrl)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(15);

            _tokenService = tokenService;
            _serviceUrl = serviceUrl;
        }

        public async Task<string> GetRemoteUser()
        {
            string endpoint = $"{_serviceUrl}/user/current";

            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

            request.Headers.Add("Authorization", await _tokenService.GetTokenAsync());

            var response = await _httpClient.SendAsync(request);

            return JsonConvert.SerializeObject(response);
        }
    }
}
