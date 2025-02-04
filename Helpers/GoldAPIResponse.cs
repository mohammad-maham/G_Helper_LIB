using GoldHelpers.Models;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;

namespace GoldHelpers.Helpers
{
    public class GoldAPIResponse
    {

        private readonly IHttpContextAccessor _contextAccessor;
        private string? ApiPath { get; set; }
        public GoldHosts Host { get; set; }
        public string? Authorization { get; set; }
        public string? Action { get; set; }
        public Method _Method { get; set; }
        public object? Data { get; set; }

        private readonly IConfiguration _config;

        public GoldAPIResponse(GoldHosts host, string action, object data, Method method = Method.Post, string? authorization = null)
        {
            string dllName = Assembly.GetExecutingAssembly().GetName().Name!;
            string? appSettingsPath = Assembly.GetExecutingAssembly().Location.Replace($"{dllName}.dll", "");
            _config = new ConfigurationBuilder().SetBasePath(appSettingsPath).AddJsonFile("goldhelper.appsettings.json").Build();
            IConfigurationSection? urlsSection = _config.GetRequiredSection("MicroservicesApiURL");

            switch (host)
            {
                case GoldHosts.Accounting:
                    ApiPath = urlsSection.GetValue<string>("Accounting")!;
                    break;
                case GoldHosts.IPG:
                    ApiPath = urlsSection.GetValue<string>("IPG")!;
                    break;
                case GoldHosts.Store:
                    ApiPath = urlsSection.GetValue<string>("Store")!;
                    break;
                case GoldHosts.Wallet:
                    ApiPath = urlsSection.GetValue<string>("Wallet")!;
                    break;
                case GoldHosts.Basket:
                    ApiPath = urlsSection.GetValue<string>("Basket")!;
                    break;
                case GoldHosts.Gateway:
                    ApiPath = urlsSection.GetValue<string>("Gateway")!;
                    break;
                case GoldHosts.Communication:
                    ApiPath = urlsSection.GetValue<string>("Communication")!;
                    break;
                default:
                    break;
            }

            Action = action;
            Authorization = authorization;
            Data = data;
            _Method = method;
            _contextAccessor = new HttpContextAccessor();
        }

        public GoldAPIResponse(string url, object? data, Method method = Method.Post, string? authorization = null)
        {
            string dllName = Assembly.GetExecutingAssembly().GetName().Name!;
            string? appSettingsPath = Assembly.GetExecutingAssembly().Location.Replace($"{dllName}.dll", "");
            _config = new ConfigurationBuilder().SetBasePath(appSettingsPath).AddJsonFile("goldhelper.appsettings.json").Build();
            Authorization = authorization;
            Data = data;
            _Method = method;
            _contextAccessor = new HttpContextAccessor();
        }

        public async Task<GoldAPIResult?> PostAsync()
        {
            GoldAPIResult? result = new();
            try
            {
                RestClient client = new(ApiPath + Action);
                RestRequest request = new()
                {
                    Method = _Method,
                    Timeout = TimeSpan.FromSeconds(20),
                };

                if (_contextAccessor != null
                    && _contextAccessor.HttpContext != null
                    && _contextAccessor.HttpContext.Request != null
                    && _contextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    AuthenticationHeaderValue? header = AuthenticationHeaderValue
                        .Parse(_contextAccessor.HttpContext.Request.Headers["Authorization"]!);
                    Authorization = header.Parameter;

                    if (!string.IsNullOrEmpty(Authorization))
                    {
                        request.AddHeader("Authorization", "Bearer " + Authorization);
                    }
                }
                else if (!string.IsNullOrEmpty(Authorization))
                {
                    request.AddHeader("Authorization", "Bearer " + Authorization);
                }

                request.AddHeader("content-type", "application/json");
                request.AddHeader("cache-control", "no-cache");

                if (Data != null)
                {
                    request.AddJsonBody(Data);
                }

                RestResponse response = await client.ExecuteAsync(request);

                if (response != null && !string.IsNullOrEmpty(response.Content) && response.StatusCode == HttpStatusCode.OK)
                {
                    result = JsonConvert.DeserializeObject<GoldAPIResult>(response.Content);
                    if (result != null && result.Message != null && result.Message.ToLower().Contains("unauthorize"))
                    {
                        result.Message = "ورود غیر مجاز لطفا دوباره وارد شوید.";
                    }
                }
                else
                {
                    if (response != null)
                    {
                        result = new GoldAPIResult()
                        {
                            StatusCode = (int)response.StatusCode,
                            Message = response.StatusDescription,
                            Data = response.Content
                        };
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                return new GoldAPIResult()
                {
                    StatusCode = -1,
                    Message = ex.Message
                };
            }
        }

        public GoldAPIResult? Post()
        {
            GoldAPIResult? result = new();
            try
            {
                RestClient client = new(ApiPath + Action);
                RestRequest request = new()
                {
                    Method = _Method,
                    Timeout = TimeSpan.FromSeconds(100),
                };

                if (_contextAccessor != null
                    && _contextAccessor.HttpContext != null
                    && _contextAccessor.HttpContext.Request != null
                    && _contextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    AuthenticationHeaderValue? header = AuthenticationHeaderValue
                        .Parse(_contextAccessor.HttpContext.Request.Headers["Authorization"]!);
                    Authorization = header.Parameter;

                    if (!string.IsNullOrEmpty(Authorization))
                    {
                        request.AddHeader("Authorization", "Bearer " + Authorization);
                    }
                }
                else if (!string.IsNullOrEmpty(Authorization))
                {
                    request.AddHeader("Authorization", "Bearer " + Authorization);
                }

                request.AddHeader("content-type", "application/json");
                request.AddHeader("accept-charset", "utf-8");

                if (Data != null)
                {
                    request.AddJsonBody(Data);
                }

                RestResponse response = client.Execute(request);

                if (response != null && !string.IsNullOrEmpty(response.Content))
                {
                    result = JsonConvert.DeserializeObject<GoldAPIResult>(response.Content);
                    if (result != null && result.Message != null && result.Message.ToLower().Contains("unauthorize"))
                    {
                        result.Message = "ورود غیر مجاز لطفا دوباره وارد شوید.";
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return new GoldAPIResult()
                {
                    StatusCode = -1,
                    Message = ex.Message
                };
            }
        }
    }
}
