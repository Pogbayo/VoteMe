using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VoteMe.Application.Events.User;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.Infrastructure.Consumers.User
{
    public class UserDeletedConsumer : BaseConsumer
    {
        private readonly IServiceScopeFactory _scopeFactory;

        protected override string QueueName => "user-deleted";

        public UserDeletedConsumer(
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

                var eventData = JsonSerializer.Deserialize<UserDeletedEvent>(json);
                if (eventData == null) return;

                using var scope = _scopeFactory.CreateScope();

                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                await notificationService.SendUserDeletedEmailAsync(
                    new List<string> { eventData.Email },
                    eventData.DisplayName,
                    eventData.DeletedAt
                );

                await unitOfWork.AuditLogs.LogAsync(
                    eventData.DeletedByUserId,
                    Domain.Enum.AuditAction.Delete,
                    $"User '{eventData.DisplayName}' (ID: {eventData.UserId}) was soft-deleted by {eventData.DeletedByUserId}"
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