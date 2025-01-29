using GoldHelpers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;

namespace GoldHelpers.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class GoldAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool isDevelopment = string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "development", StringComparison.InvariantCultureIgnoreCase);

            StringValues headerValues = context.HttpContext.Request.Headers[HeaderNames.Authorization];
            AuthenticationHeaderValue.TryParse(headerValues, out AuthenticationHeaderValue? headerValue);

            if (!isDevelopment)
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
                    // Get read appsettings.json of helpers settings
                    string dllName = Assembly.GetExecutingAssembly().GetName().Name!;
                    string? appSettingsPath = Assembly.GetExecutingAssembly().Location.Replace($"{dllName}.dll", "");
                    IConfigurationRoot? configs = new ConfigurationBuilder().SetBasePath(appSettingsPath).AddJsonFile("goldhelper.appsettings.json").Build();
                    IConfigurationSection? urlsSection = configs.GetSection("ApplicationURL");
                    string? liveUrl = urlsSection.GetValue<string>("AccountingLive")!;
                    string? preLiveUrl = urlsSection.GetValue<string>("AccountingPreLive")!;
                    string host = !isDevelopment ? liveUrl : preLiveUrl;

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

                        // Check response body
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
}