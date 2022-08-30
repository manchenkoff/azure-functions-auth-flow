using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ClientApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace ClientApp
{
    public class User
    {
        private readonly IUserService _userService;

        public User(IUserService userService)
        {
            _userService = userService;
        }

        [FunctionName(nameof(Current))]
        [OpenApiOperation(operationId: "GetCurrentUser", tags: new[] { "User" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Current([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/current")] HttpRequest req, ILogger logger)
        {
            logger.LogInformation("RequestContext-Headers: {data}", JsonConvert.SerializeObject(req.Headers));
            logger.LogInformation("RequestContext-User-Identity: {data}", JsonConvert.SerializeObject(req.HttpContext.User?.Identity?.ToString()));
            logger.LogInformation("RequestContext-User-Claims: {data}", JsonConvert.SerializeObject(req.HttpContext.User.Claims.Select(clm => $"{clm.ValueType}-{clm.Value}")));

            var response = await _userService.GetRemoteUser();

            return new OkObjectResult(response);
        }
    }
}

