using FastEndpoints;
using FastEndpoints.Swagger;

namespace ProducerTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services
                   .AddFastEndpoints()
                   .SwaggerDocument();

            var app = builder.Build();

            app.UseFastEndpoints()
               .UseSwaggerGen();

            app.Run();
        }
    }
}
