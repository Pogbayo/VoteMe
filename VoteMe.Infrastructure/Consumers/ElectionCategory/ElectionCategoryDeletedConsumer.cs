using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VoteMe.Application.Events.ElectionCategory;
using VoteMe.Application.Interface.IRepositories;

namespace VoteMe.Infrastructure.Consumers.ElectionCategory
{
    public class ElectionCategoryDeletedConsumer : BaseConsumer
    {
        private readonly IServiceScopeFactory _scopeFactory;
        protected override string QueueName => "election-category-deleted";

        public ElectionCategoryDeletedConsumer(
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
                var eventData = JsonSerializer.Deserialize<ElectionCategoryDeletedEvent>(json);

                if (eventData == null) return;

                using var scope = _scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                await unitOfWork.AuditLogs.LogAsync(
                    eventData.DeletedByUserId,
                    Domain.Enum.AuditAction.Delete,
                    $"ElectionCategory '{eventData.ElectionCategoryName}' deleted from election '{eventData.ElectionName}'"
                );

                await unitOfWork.SaveChangesAsync();
                Channel.BasicAck(args.DeliveryTag, false);
            };

            Channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }
    }
}