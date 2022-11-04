using System.Linq;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ServerApp.Filters;
using ServerApp.Models;

namespace ServerApp.Functions
{
    public class User
    {
        [FunctionName(nameof(Current))]
        [OpenApiOperation(operationId: nameof(Current), tags: new[] { "User" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserModel), Description = "Current user details")]
        [OpenApiSecurity("Token", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        public IActionResult Current([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/current")] HttpRequest req, ClaimsPrincipal principal, ILogger logger)
        {
            var user = ExtractUserFromPrincipal(principal);

            return new OkObjectResult(user);
        }

        [Authorize]
        [FunctionName(nameof(Restricted))]
        [OpenApiOperation(operationId: nameof(Restricted), tags: new[] { "User" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserModel), Description = "Current user details with filter check")]
        [OpenApiSecurity("Token", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        public IActionResult Restricted([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/restricted")] HttpRequest req, ClaimsPrincipal principal, ILogger logger)
        {
            var user = ExtractUserFromPrincipal(principal);

            return new OkObjectResult(user);
        }

        private UserModel ExtractUserFromPrincipal(ClaimsPrincipal principal)
        {
            return new UserModel()
            {
                Username = principal.Claims.FirstOrDefault(clm => clm.Type == "name")?.Value
                    ?? principal.Claims.FirstOrDefault(clm => clm.Type == "appid")?.Value
                    ?? string.Empty,

                Email = principal.Claims.FirstOrDefault(clm => clm.Type == ClaimTypes.Email)?.Value
                    ?? principal.Claims.FirstOrDefault(clm => clm.Type == "roles")?.Value
                    ?? string.Empty,

                Application = "ServerApp",
            };
        }
    }
}

