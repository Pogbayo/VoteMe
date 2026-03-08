using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VoteMe.Application.Events.Election;

namespace VoteMe.Infrastructure.Consumers.Election
{
    public class ElectionUpdatedConsumer : BaseConsumer
    {
        protected override string QueueName => "election-updated";

        public ElectionUpdatedConsumer(
            IConfiguration configuration) : base(configuration)
        {
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(Channel);

            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var eventData = JsonSerializer.Deserialize<ElectionUpdatedEvent>(json);

                if (eventData == null) return;

                Channel.BasicAck(args.DeliveryTag, false);
            };

            Channel.BasicConsume(
                queue: QueueName,
                autoAck: false,
                consumer: consumer
            );

            return Task.CompletedTask;
        }
    }
}