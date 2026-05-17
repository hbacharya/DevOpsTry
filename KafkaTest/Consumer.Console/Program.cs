using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Consumer.Console
{
    internal static class Program
    {
        private static IConfigurationRoot configuration;
        static IConsumer<string, string> consumer;

        private static void Main(string[] args)
        {
            // read from appsettings.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            configuration = builder.Build();

            Subscribe();

            while (true)
            {
                var result = consumer.Consume(3000);
                if (result is null)
                {
                    continue;
                }

                var message = result.Message.Value;

                // produce the same message to destination topic
                var servers = configuration.GetSection("Kafka:BootstrapServers").Value;
                var groupInstanceId = configuration.GetSection("Kafka:GroupInstanceId").Value;
                var producerConfig = new ProducerConfig { BootstrapServers = servers };
                using (var producer = new ProducerBuilder<string, string>(producerConfig).Build())
                {
                    var destinationTopic = configuration.GetSection("Kafka:DestinationTopic").Value;
                    // append timestamp to the message
                    var timestamp = DateTime.UtcNow.ToString("o"); // ISO 8601 format
                    message = $"{timestamp} - {groupInstanceId} - {message}";
                    producer.ProduceAsync(destinationTopic, new Message<string, string>
                    {
                        Key = result.Message.Key,
                        Value = message
                    })
                        .GetAwaiter()
                        .GetResult();
                }

                consumer.Commit(result);
            }
        }

        private static void Subscribe()
        {
            var groupId = "test-console-consumer-group";
            var servers = configuration.GetSection("Kafka:BootstrapServers").Value;
            var topic = configuration.GetSection("Kafka:SourceTopic").Value;
            var groupInstanceId = configuration.GetSection("Kafka:GroupInstanceId").Value;

            System.Console.WriteLine($"Server: {servers}");
            System.Console.WriteLine($"Subscribing to topic: {topic}. Group Instance ID: {groupInstanceId}.");
            
            var config = new ConsumerConfig
            {
                BootstrapServers = servers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                GroupInstanceId = groupInstanceId,
                ClientId = groupInstanceId
            };

            consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe(topic);
        }
    }
}