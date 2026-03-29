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
    public class OrganizationCreatedConsumer : BaseConsumer
    {
        private readonly IServiceScopeFactory _scopeFactory;
        protected override string QueueName => "organization-created";

        public OrganizationCreatedConsumer(
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
                var eventData = JsonSerializer.Deserialize<OrganizationCreatedEvent>(json);

                if (eventData == null) return;

                using var scope = _scopeFactory.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                await notificationService.SendOrganizationCreatedEmailAsync(
                    new List<string> { eventData.AdminEmail },
                    eventData.AdminDisplayName,
                    eventData.OrganizationName,
                    eventData.UniqueKey
                );

                await unitOfWork.AuditLogs.LogAsync(
                    eventData.AdminUserId,
                    Domain.Enum.AuditAction.Create,
                    $"Organization '{eventData.OrganizationName}' created by '{eventData.AdminDisplayName}'"
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