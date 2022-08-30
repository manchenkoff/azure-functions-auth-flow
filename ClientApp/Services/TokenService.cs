using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ClientApp.Services
{
    public class TokenService : ITokenService
    {
        private readonly string _applicationId;
        private readonly string _applicationSecret;
        private readonly string _tenantId;
        private readonly List<string> _scopes;

        public TokenService(string appId, string appSecret, string tenant, string audience)
        {
            _applicationId = appId;
            _applicationSecret = appSecret;
            _tenantId = tenant;

            _scopes = new List<string>()
            {
                $"{audience}/.default",
            };
        }

        public async Task<string> GetTokenAsync()
        {
            var builder = ConfidentialClientApplicationBuilder
                .Create(_applicationId)
                .WithAuthority($"https://login.microsoftonline.com/{_tenantId}", false)
                .WithClientSecret(_applicationSecret);

            var clientApp = builder.Build();

            var authResult = await this.GetAuthResult(clientApp);

            return authResult.CreateAuthorizationHeader();
        }

        private async Task<AuthenticationResult> GetAuthResult(IConfidentialClientApplication application)
        {
            // Client version
            var parameterBuilder = application.AcquireTokenForClient(_scopes);

            // On behalf version
            //var userAssertion = new UserAssertion("USER_TOKEN_FROM_HEADERS_GOES_HERE");
            //var parameterBuilderObo = application.AcquireTokenOnBehalfOf(scopes, userAssertion);

            return await parameterBuilder.ExecuteAsync();
        }
    }
}
