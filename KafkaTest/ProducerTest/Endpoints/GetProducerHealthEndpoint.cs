using FastEndpoints;

namespace ProducerTest.Endpoints
{
    public class GetHealthEndpoint : EndpointWithoutRequest<string>
    {
        public override void Configure()
        {
            Get("/health");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            await Send.OkAsync($"Producer Healthy at {DateTime.UtcNow}", cancellation: ct);
        }
    }
}