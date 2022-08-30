using ClientApp.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Net.Http;

[assembly: FunctionsStartup(typeof(ClientApp.Startup))]

namespace ClientApp
{
    internal class OpenApiConfigurationOptions : DefaultOpenApiConfigurationOptions
    {
        public override OpenApiInfo Info { get; set; } = new OpenApiInfo
        {
            Version = "1.0.0",
            Title = "Test OpenAPI",
            Description = "Server API specification",
            License = new OpenApiLicense
            {
                Name = "MIT",
                Url = new Uri("http://opensource.org/licenses/MIT"),
            }
        };

        public override OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V3;
    }

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder
                .Services
                .AddHttpClient()
                .AddScoped<ITokenService>(s => {
                    var appId = Environment.GetEnvironmentVariable("AAD:AppId");
                    var appSecret = Environment.GetEnvironmentVariable("AAD:AppSecret");
                    var tenant = Environment.GetEnvironmentVariable("AAD:Tenant");
                    var audience = Environment.GetEnvironmentVariable("AAD:Audience");

                    return new TokenService(appId, appSecret, tenant, audience);
                })
                .AddScoped<IUserService>(s =>
                {
                    var httpClientFactory = s.GetService<IHttpClientFactory>();
                    var tokenService = s.GetService<ITokenService>();
                    var serviceBaseUrl = Environment.GetEnvironmentVariable("UserServiceURL");

                    return new UserService(httpClientFactory, tokenService, serviceBaseUrl);
                });
        }
    }
}
