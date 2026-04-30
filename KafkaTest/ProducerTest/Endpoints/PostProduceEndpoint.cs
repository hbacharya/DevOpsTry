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
            var bootstrapServers = req.Servers; //_configuration["Kafka:BootstrapServers"]!;
            var topic = req.Topic; //_configuration["Kafka:Topic"]!;

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = bootstrapServers
            };

            using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();

            var deliveryResult = await producer.ProduceAsync(
                topic,
                new Message<Null, string> { Value = req.Message },
                ct);

            var response = new ProducerResponse
            {
                Offset = deliveryResult.Offset.Value
            };

            await Send.OkAsync(response, ct);
        }
    }
}