using RestSharp;
namespace GoldHelpers.Helpers
{
    public class SMSSender
    {
        private readonly IConfiguration _config;

        public SMSSender()
        {
            _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        }

        public static void SendSMS(string message, long mobile)
        {
            string baseUrl = _config.GetSection("ProjectURLs").Get<string>();
            RestClient client = new($"{baseUrl}/");
            RestRequest request = new()
            {
                Method = Method.Post
            };
        }
    }
}
