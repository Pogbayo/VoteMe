using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VoteMe.Application.Events.Voting;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.Infrastructure.Consumers.Voting
{
    public class VoteCastConsumer : BaseConsumer
    {
        private readonly IServiceScopeFactory _scopeFactory;
        protected override string QueueName => "vote-cast";

        public VoteCastConsumer(
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
                var eventData = JsonSerializer.Deserialize<VoteCastEvent>(json);

                if (eventData == null) return;

                using var scope = _scopeFactory.CreateScope();
                var notificationService = scope.ServiceProvider
                    .GetRequiredService<INotificationService>();
                var cacheService = scope.ServiceProvider
                    .GetRequiredService<ICacheService>();

                await cacheService.RemoveAsync($"election-results-{eventData.ElectionId}");

                await notificationService.SendVoteConfirmationEmailAsync(
                    new List<string> { eventData.VoterEmail },
                    eventData.VoterFullName,
                    eventData.ElectionTitle,
                    eventData.CandidateName
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