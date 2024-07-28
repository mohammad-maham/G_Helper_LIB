using Microsoft.AspNetCore.Mvc.Filters;
using RestSharp;

namespace GoldHelpers.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class GoldUserInfoAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            return;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            string host = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build()
                .GetSection("ProjectURLs")
                .GetValue<string>("Accounting")!;

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
                request.AddBody(new { context = context });

                // Send SMS
                RestResponse response = client.ExecutePost(request);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
