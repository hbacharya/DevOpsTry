using Confluent.Kafka;
using FastEndpoints;

namespace ProducerTest.Endpoints
{
    public class PostProduceRequest
    {
        public string Message { get; set; } = string.Empty;
        public string Servers { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
    }

    public class ProducerResponse
    {
        public long Offset { get; set; }
    }

    public class PostProduceEndpoint : Endpoint<PostProduceRequest, ProducerResponse>
    {
        private readonly IConfiguration _configuration;

        public PostProduceEndpoint(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override void Configure()
        {
            Post("/produce");
            AllowAnonymous();
        }

        public override async Task HandleAsync(PostProduceRequest req, CancellationToken ct)
        {
            // await Send.OkAsync(new ProducerResponse
            // {
            //     Offset = -1 // Placeholder, actual offset will be set after successful production
            // }, ct);
            // return;
            
            var bootstrapServers = req.Servers; //_configuration["Kafka:BootstrapServers"]!;
            var topic = req.Topic; //_configuration["Kafka:Topic"]!;

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                MessageMaxBytes = 20971520
            };

            using var producer = new ProducerBuilder<string, string>(producerConfig).Build();

            var filePath = @"C:\Hardik\Learn\devops\DevOpsTry\KafkaTest\ProducerTest\bin\Debug\net10.0\NSwag.AspNetCore.dll";
            var bytes = File.ReadAllBytes(filePath);
            var fileText = Convert.ToBase64String(bytes);
            var reqMessage = fileText; //req.Message;

            try
            {
                var deliveryResult = await producer.ProduceAsync(
                    topic,
                    new Message<string, string>
                        { Key = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"), Value = reqMessage },
                    ct);

                var response = new ProducerResponse
                {
                    Offset = deliveryResult.Offset.Value
                };

                await Send.OkAsync(response, ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}