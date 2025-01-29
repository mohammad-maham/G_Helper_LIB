using System.Net;

namespace GoldHelpers.Middleware
{
    public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                APIResponse response = _env.IsDevelopment()
                    ? new APIResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace!.ToString())
                    : new APIResponse((int)HttpStatusCode.InternalServerError);

                /*JsonSerializerOptions options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                string json = JsonSerializer.Serialize(response, options);*/

                await context.Response.WriteAsJsonAsync(response).ConfigureAwait(false);
            }
        }
    }
}
