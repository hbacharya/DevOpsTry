using FastEndpoints;

namespace ConsumerTest.Endpoints
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
            await Send.OkAsync($"Consumer Healthy at {DateTime.UtcNow}", cancellation: ct);
        }
    }
}