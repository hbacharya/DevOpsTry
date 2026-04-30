using Confluent.Kafka;
using ConsumerTest.Endpoints;

namespace ConsumerTest.Services
{
    public interface IKafkaConsumerService
    {
        Task<string?> ConsumeOnceAsync(int timeoutMs, CancellationToken ct);
        void StartIfNeeded(PostConsumerRequest request);
    }

    public sealed class KafkaConsumerService : IKafkaConsumerService, IDisposable
    {
        private IConsumer<Null, string> _consumer = null;
        private readonly SemaphoreSlim _lock = new(1, 1);
        private bool _isStarted = false;
        
        public KafkaConsumerService()
        {
        }

        public async Task<string?> ConsumeOnceAsync(int timeoutMs, CancellationToken ct)
        {
            await _lock.WaitAsync(ct);
            try
            {
                var result = _consumer.Consume(timeoutMs);
                if (result is null)
                    return null;

                _consumer.Commit(result);
                return $"Message consumed at offset {result.Offset}: {result.Message.Value}";
            }
            finally
            {
                _lock.Release();
            }
        }

        public void StartIfNeeded(PostConsumerRequest request)
        {
            if (_isStarted)
            {
                return;
            }

            var groupId = "test-consumer-group";

            var config = new ConsumerConfig
            {
                BootstrapServers = request.Servers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            _consumer = new ConsumerBuilder<Null, string>(config).Build();
            _consumer.Subscribe(request.Topic);

            _isStarted = true;
        }

        public void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();
            _lock.Dispose();
        }
    }
}