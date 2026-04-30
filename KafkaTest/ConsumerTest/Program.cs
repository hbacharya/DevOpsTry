using ConsumerTest.Services;
using FastEndpoints;
using FastEndpoints.Swagger;

namespace ConsumerTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services
                   .AddFastEndpoints()
                   .SwaggerDocument(o =>
                   {
                       o.DocumentSettings = s =>
                       {
                           s.Title = "ConsumerTest API";
                           s.Version = "v4";
                       };
                   });

            builder.Services.AddSingleton<IKafkaConsumerService, KafkaConsumerService>();

            var app = builder.Build();
            app.UseFastEndpoints().UseSwaggerGen();

            app.Run();
        }
    }
}
