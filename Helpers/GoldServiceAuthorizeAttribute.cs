using GoldHelpers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

namespace GoldHelpers.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class GoldServiceAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            StringValues headerValues = context.HttpContext.Request.Headers[HeaderNames.Authorization];
            AuthenticationHeaderValue.TryParse(headerValues, out AuthenticationHeaderValue? headerValue);

            bool isDevelopment = string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "development", StringComparison.InvariantCultureIgnoreCase);

            string reqPath = context.HttpContext.Request.Path.Value ?? "";
            string callerService = reqPath.Split("/")[3] ?? "";
            string[] srvTrust = { "Auth", "SendAuthOTP", "VerifyAuthOTP" };

            if (!isDevelopment || !srvTrust.Contains(callerService))
            {
                if (headerValue == null || headerValue.Parameter == null)
                {
                    // No Athuorization header, Unauthorized!
                    context.Result = new JsonResult(new { message = "Unauthorized!" })
                    { StatusCode = StatusCodes.Status401Unauthorized };
                }
                else
                {
                    string token = headerValue.Parameter;

                    try
                    {
                        GoldAPIResult? result = new GoldAPIResponse(GoldHosts.Accounting, "/api/Attributes/GetServiceAuthorize", new { Token = token }).Post();

                        if (result != null && result.Data == "false")
                        {
                            context.Result = new JsonResult(new { message = "Unauthorized!" })
                            { StatusCode = StatusCodes.Status401Unauthorized };
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
    }
}