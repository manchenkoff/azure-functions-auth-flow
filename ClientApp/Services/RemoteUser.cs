using ClientApp.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ClientApp.Services
{
    public class RemoteUser : IRemoteUser
    {
        private const string endpoint = "/api/user/current";

        private readonly HttpClient _httpClient;
        private readonly ITokenProvider _tokenProvider;


        public RemoteUser(IHttpClientFactory httpClientFactory, string remoteHost, ITokenProvider tokenProvider)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
            _httpClient.BaseAddress = new Uri(remoteHost);

            _tokenProvider = tokenProvider;
        }

        public async Task<UserModel> Current(ILogger logger)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            var token = await _tokenProvider.GetToken();

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Server returned a response with {(int)response.StatusCode} code.");
            }

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<UserModel>(content);
        }

        public async Task<UserModel> CurrentOnBehalfOf(string userToken, ILogger logger)
        {
            if (string.IsNullOrEmpty(userToken))
            {
                throw new ArgumentNullException("User token must not be empty");
            }

            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            // Pass current X-MS-TOKEN-AAD-ID-TOKEN from the request
            var token = await _tokenProvider.GetTokenObo(userToken);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Server returned a response with {(int)response.StatusCode} code.");
            }

            var content = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<UserModel>(content);
        }
    }
}
