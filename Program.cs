using GoldHelpers.Helpers;
using GoldHelpers.Middleware;

namespace GoldHelpers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.Converters.Add(new CustomJsonConverterForType());
            });
            WebApplication app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();
            app.Run();
        }
    }
}
