using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VoteMe.Application.Events.Election;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.Infrastructure.Consumers.Election
{
    public class ElectionClosedConsumer : BaseConsumer
    {
        private readonly IServiceScopeFactory _scopeFactory;
        protected override string QueueName => "election-closed";

        public ElectionClosedConsumer(
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory) : base(configuration)
        {
            _scopeFactory = scopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(Channel);

            consumer.Received += async (sender, args) =>
            {
                var body = args.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);
                var eventData = JsonSerializer.Deserialize<ElectionClosedEvent>(json);

                if (eventData == null) return;

                using var scope = _scopeFactory.CreateScope();
                var notificationService = scope.ServiceProvider
                    .GetRequiredService<INotificationService>();

                await notificationService.SendElectionClosedEmailAsync(
                    eventData.MemberEmails,
                    eventData.ElectionTitle,
                    eventData.OrganizationName
                );

                await notificationService.SendElectionResultsEmailAsync(
                    eventData.MemberEmails,
                    eventData.ElectionTitle,
                    eventData.WinnerName,
                    eventData.TotalVotes
                );

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