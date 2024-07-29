using GoldHelpers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Net.Http.Headers;

namespace GoldHelpers.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class GoldAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            StringValues headerValues = context.HttpContext.Request.Headers[HeaderNames.Authorization];
            AuthenticationHeaderValue.TryParse(headerValues, out AuthenticationHeaderValue? headerValue);
            if (headerValue == null || headerValue.Parameter == null)
            {
                context.Result = new JsonResult(new { message = "Unauthorized!" })
                { StatusCode = StatusCodes.Status401Unauthorized };
            }
            else
            {
                string token = headerValue.Parameter;
                bool isProductionMode = string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "development", StringComparison.InvariantCultureIgnoreCase);

                string host = isProductionMode ? new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("appsettings.json")
                               .Build()
                               .GetSection("ProjectURLs")
                               .GetValue<string>("Accounting")! : "http://localhost:5288";

                try
                {
                    // BaseURL
                    RestClient client = new($"{host}/api/Attributes/GetAuthorize");
                    RestRequest request = new()
                    {
                        Method = Method.Post
                    };

                    // Headers
                    request.AddHeader("content-type", "application/json");
                    request.AddHeader("cache-control", "no-cache");

                    // Body
                    request.AddJsonBody(new { Token = token });

                    // Send SMS
                    RestResponse response = client.ExecutePost(request);

                    if (response != null && response.StatusCode == HttpStatusCode.BadRequest && !string.IsNullOrEmpty(response.Content))
                    {
                        ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(response.Content)!;
                        if (apiResponse != null && apiResponse.Data == "false")
                        {
                            context.Result = new JsonResult(new { message = "Unauthorized!" })
                            { StatusCode = StatusCodes.Status401Unauthorized };
                        }
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