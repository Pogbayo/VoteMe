using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VoteMe.Application.Events.Election;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.Infrastructure.Consumers.Election
{
    public class ElectionOpenedConsumer : BaseConsumer
    {
        private readonly IServiceScopeFactory _scopeFactory;
        protected override string QueueName => "election-opened";

        public ElectionOpenedConsumer(
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
                var eventData = JsonSerializer.Deserialize<ElectionOpenedEvent>(json);

                if (eventData == null) return;

                using var scope = _scopeFactory.CreateScope();
                var notificationService = scope.ServiceProvider
                    .GetRequiredService<INotificationService>();
                var unitOfWork = scope.ServiceProvider
                    .GetRequiredService<IUnitOfWork>();

                await notificationService.SendElectionOpenedEmailAsync(
                    eventData.MemberEmails,
                    eventData.ElectionName,
                    eventData.OrganizationName
                );

                await unitOfWork.AuditLogs.LogAsync(
                    eventData.OpenedByUserId,
                    "ElectionOpened",
                    "Election",
                    $"Election '{eventData.ElectionName}' opened in '{eventData.OrganizationName}'"
                );
                await unitOfWork.SaveChangesAsync();

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