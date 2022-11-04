using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using ClientApp.Models;
using ClientApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace ClientApp.Functions
{
    public class User
    {
        private readonly IRemoteUser _remoteUserService;

        public User(IRemoteUser remoteUserService)
        {
            _remoteUserService = remoteUserService;
        }

        [FunctionName(nameof(Current))]
        [OpenApiOperation(operationId: nameof(Current), tags: new[] { "User" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserModel), Description = "Current user details")]
        public IActionResult Current([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/current")] HttpRequest req, ClaimsPrincipal principal, ILogger logger)
        {
            var user = ExtractUserFromPrincipal(principal);

            return new OkObjectResult(user);
        }

        [FunctionName(nameof(Remote))]
        [OpenApiOperation(operationId: nameof(Remote), tags: new[] { "User" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserModel), Description = "Remote user details by the application token")]
        public async Task<IActionResult> Remote([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/remote")] HttpRequest req, ClaimsPrincipal principal, ILogger logger)
        {
            var user = await _remoteUserService.Current(logger);

            return new OkObjectResult(user);
        }

        [FunctionName(nameof(RemoteOnBehalf))]
        [OpenApiOperation(operationId: nameof(RemoteOnBehalf), tags: new[] { "User" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UserModel), Description = "Remote user details by the current user token")]
        public async Task<IActionResult> RemoteOnBehalf([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/remote/onbehalf")] HttpRequest req, ClaimsPrincipal principal, ILogger logger)
        {
            var currentUserToken = req.Headers["X-MS-TOKEN-AAD-ID-TOKEN"];
            var user = await _remoteUserService.CurrentOnBehalfOf(currentUserToken, logger);

            return new OkObjectResult(user);
        }

        private UserModel ExtractUserFromPrincipal(ClaimsPrincipal principal)
        {
            return new UserModel()
            {
                Username = principal.Claims.FirstOrDefault(clm => clm.Type == "name")?.Value ?? string.Empty,
                Email = principal.Claims.FirstOrDefault(clm => clm.Type == ClaimTypes.Email)?.Value ?? string.Empty,
                Application = "ClientApp",
            };
        }
    }
}

