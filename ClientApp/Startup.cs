using ClientApp.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

[assembly: FunctionsStartup(typeof(ClientApp.Startup))]

namespace ClientApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder
                .Services
                .AddHttpClient()
                .AddScoped<ITokenProvider>(s => 
                {
                    var applicationId = Environment.GetEnvironmentVariable("AAD:AppId");
                    var applicationSecret = Environment.GetEnvironmentVariable("AAD:AppKey");
                    var tenant = Environment.GetEnvironmentVariable("AAD:TenantId");
                    var audience = Environment.GetEnvironmentVariable("ServerApp:Audience");

                    return new AadTokenProvider(applicationId, applicationSecret, tenant, audience);
                })
                .AddScoped<IRemoteUser>(s =>
                {
                    var httpClientFactory = s.GetService<IHttpClientFactory>();
                    var tokenProvider = s.GetService<ITokenProvider>();
                    var logger = s.GetService<ILogger>();

                    var remoteHost = Environment.GetEnvironmentVariable("ServerApp:RemoteHost");

                    return new RemoteUser(httpClientFactory, remoteHost, tokenProvider);
                });
        }
    }
}
