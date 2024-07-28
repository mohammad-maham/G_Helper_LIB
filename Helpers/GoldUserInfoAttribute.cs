using Accounting.Helpers;
using Accounting.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
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

            UserInfoAttribute userInfoAttribute = new UserInfoAttribute();
            userInfoAttribute.OnActionExecuting(context);

            try
            {
                // BaseURL
                RestClient client = new($"{host}/api/SMTP/SendOTPSMS");
                RestRequest request = new()
                {
                    Method = Method.Post
                };

                // Parameters
                //request.AddJsonBody(new { Mobile = sms.Destination!.Value.ToString(), OTP = sms.Options!.Message });

                // Headers
                request.AddHeader("content-type", "application/json");
                request.AddHeader("cache-control", "no-cache");

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
