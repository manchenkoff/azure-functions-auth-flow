using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace ServerApp
{
    public class User
    {
        [FunctionName(nameof(Current))]
        [OpenApiOperation(operationId: "GetCurrentUser", tags: new[] { "User" })]
        [OpenApiSecurity("Token", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
        public IActionResult Current([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/current")] HttpRequest req, ILogger logger)
        {
            logger.LogInformation("RequestContext-Headers: {data}", JsonConvert.SerializeObject(req.Headers));
            logger.LogInformation("RequestContext-User-Identity: {data}", JsonConvert.SerializeObject(req.HttpContext.User?.Identity?.ToString()));
            logger.LogInformation("RequestContext-User-Claims: {data}", JsonConvert.SerializeObject(req.HttpContext.User.Claims.Select(clm => $"{clm.ValueType}-{clm.Value}")));

            this.HandleJwtToken(req, logger);

            return new OkObjectResult("OK");
        }

        private void HandleJwtToken(HttpRequest req, ILogger logger)
        {
            var bearerToken = req.Headers["Authorization"].ToString().Split(' ').LastOrDefault();

            if (string.IsNullOrEmpty(bearerToken))
            {
                logger.LogInformation("No Bearer token in request");

                return;
            }

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(bearerToken);

            logger.LogInformation("TokenContext-Claims: {data}", token.RawData);
            logger.LogInformation("TokenContext-Issuer: {data}", token.Issuer);
            logger.LogInformation("TokenContext-Actor: {data}", token.Actor);
            logger.LogInformation("TokenContext-Audiences: {data}", token.Audiences);
            logger.LogInformation("TokenContext-Claims: {data}", token.Claims.Select(clm => $"{clm.ValueType}-{clm.Value}"));
            logger.LogInformation("TokenContext-Subject: {data}", token.Subject);
            logger.LogInformation("TokenContext-Id: {data}", token.Id);
            logger.LogInformation("TokenContext-InnerToken: {data}", token.InnerToken);
            logger.LogInformation("TokenContext-Payload: {data}", token.Payload);
            logger.LogInformation("TokenContext-ValidTo: {data}", token.ValidTo);
        }
    }
}

