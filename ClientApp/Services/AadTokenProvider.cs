using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClientApp.Services
{
    public class AadTokenProvider : ITokenProvider
    {
        private const string LOGIN_ENDPOINT = "https://login.microsoftonline.com";

        private readonly string _applicationId;
        private readonly string _applicationSecret;
        private readonly string _tenantId;
        private readonly string _audience;

        public AadTokenProvider(string applicationId, string applicationSecret, string tenantId, string audience)
        {
            _applicationId = applicationId;
            _applicationSecret = applicationSecret;
            _tenantId = tenantId;
            _audience = audience;
        }

        public async Task<string> GetToken()
        {
            var authRequest = GetConfidentialApplication().AcquireTokenForClient(CreateAppScopes());
            var authResponse = await authRequest.ExecuteAsync();

            return authResponse.AccessToken;
        }

        public async Task<string> GetTokenObo(string authToken)
        {
            var authRequest = GetConfidentialApplication().AcquireTokenOnBehalfOf(CreateAppScopes(), new UserAssertion(authToken));
            var authResponse = await authRequest.ExecuteAsync();

            return authResponse.AccessToken;
        }

        private IConfidentialClientApplication GetConfidentialApplication()
        {
            return ConfidentialClientApplicationBuilder
                .Create(_applicationId)
                .WithClientSecret(_applicationSecret)
                .WithAuthority(CreateAuthorityUrl(), false)
                .WithTenantId(_tenantId)
                .Build();
        }

        private string CreateAuthorityUrl()
        {
            return $"{LOGIN_ENDPOINT}/{_tenantId}";
        }

        private IEnumerable<string> CreateAppScopes()
        {
            return new string[]
            {
                $"{this._audience}/.default",
            };
        }
    }
}
