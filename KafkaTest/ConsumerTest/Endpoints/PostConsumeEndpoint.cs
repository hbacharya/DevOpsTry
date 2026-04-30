using ConsumerTest.Services;
using FastEndpoints;

namespace ConsumerTest.Endpoints
{
    public class PostConsumerRequest
    {
        public string Servers { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public int TimeOut { get; set; } = 15000;
    }

    public class PostConsumeEndpoint : Endpoint<PostConsumerRequest, string>
    {
        private readonly IKafkaConsumerService _consumerService;

        public PostConsumeEndpoint(IKafkaConsumerService consumerService)
        {
            _consumerService = consumerService;
        }

        public override void Configure()
        {
            Post("/consume");
            AllowAnonymous();
        }

        public override async Task HandleAsync(PostConsumerRequest req, CancellationToken ct)
        {
            _consumerService.StartIfNeeded(req);
            var message = await _consumerService.ConsumeOnceAsync(req.TimeOut, ct);
            await Send.OkAsync(message ?? "No message consumed", ct);
        }
    }
}