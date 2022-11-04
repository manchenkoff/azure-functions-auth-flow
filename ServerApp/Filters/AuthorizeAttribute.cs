using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ServerApp.Filters
{
    [Obsolete]
    public class AuthorizeAttribute : FunctionInvocationFilterAttribute
    {
        private const string RequestArgumentKey = "req";
        private const string TokenKey = "Authorization";
        private const string TokenConfigUrl = "https://login.microsoftonline.com/common/.well-known/openid-configuration";

        public override async Task OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
        {
            try
            {
                var request = (HttpRequest)executingContext.Arguments[RequestArgumentKey];
                var token = request.Headers[TokenKey].ToString().Split(' ').LastOrDefault();

                var principal = await this.AuthorizeToken(token);

                request.HttpContext.User.AddIdentities(principal.Identities);
            }
            catch (Exception ex)
            {
                throw new FunctionInvocationException("Unauthorized", ex);
            }
        }

        private async Task<ClaimsPrincipal> AuthorizeToken(string token)
        {
            var validator = new JwtSecurityTokenHandler();
            var validationParameters = await GetTokenConfiguration();

            var claimsPrincipal = validator.ValidateToken(token, validationParameters, out _);

            return claimsPrincipal;
        }

        private async Task<TokenValidationParameters> GetTokenConfiguration()
        {
            var tokenConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                TokenConfigUrl,
                new OpenIdConnectConfigurationRetriever());

            var tokenConfiguration = await tokenConfigurationManager.GetConfigurationAsync();

            var appId = Environment.GetEnvironmentVariable("AAD:AppId");
            var tenantId = Environment.GetEnvironmentVariable("AAD:TenantId");

            return new TokenValidationParameters
            {
                ValidAudiences = new string[] { appId },
                ValidIssuers = new string[] { $"https://sts.windows.net/{tenantId}/" },

                ValidateAudience = true,
                ValidateIssuer = true,

                IssuerSigningKeys = tokenConfiguration.SigningKeys,
            };
        }
    }
}
