using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VoteMe.Application.Events.Organization;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.Infrastructure.Consumers.Organization
{
    public class OrganizationDeletedConsumer : BaseConsumer
    {
        private readonly IServiceScopeFactory _scopeFactory;

        protected override string QueueName => "organization-deleted";

        public OrganizationDeletedConsumer(
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory)
            : base(configuration)
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

                var eventData = JsonSerializer.Deserialize<OrganizationDeletedEvent>(json);
                if (eventData == null) return;

                using var scope = _scopeFactory.CreateScope();

                var notificationService = scope.ServiceProvider
                    .GetRequiredService<INotificationService>();

                var unitOfWork = scope.ServiceProvider
                    .GetRequiredService<IUnitOfWork>();

                if (eventData.MemberEmails?.Any() == true)
                {
                    await notificationService.SendOrganizationDeletedNotificationAsync(
                        eventData.OrganizationName,
                        eventData.MemberEmails,
                        eventData.DeletedAt);
                }

                await unitOfWork.AuditLogs.LogAsync(
                    eventData.DeletedByUserId,
                    "OrganizationDeleted",
                    "Organization",
                    $"Organization '{eventData.OrganizationName}' (ID: {eventData.OrganizationId}) " +
                    $"was deleted by user {eventData.DeletedByUserId}. " +
                    $"{eventData.MemberEmails?.Count ?? 0} members were notified.");

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