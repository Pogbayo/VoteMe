using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VoteMe.Application.Events.Candidate;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;
using VoteMe.Domain.Enum;

namespace VoteMe.Infrastructure.Consumers.Candidate
{
    public class CandidateAddedConsumer : BaseConsumer
    {
        private readonly IServiceScopeFactory _scopeFactory;
        protected override string QueueName => "candidate-added";

        public CandidateAddedConsumer(
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
                var eventData = JsonSerializer.Deserialize<CandidateAddedEvent>(json);

                if (eventData == null) return;

                using var scope = _scopeFactory.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var candidateName = eventData.CandidateDisplayName
                    ?? $"{eventData.CandidateFirstName} {eventData.CandidateLastName}";

                await notificationService.SendCandidateAddedEmailAsync(
                    eventData.MemberEmails,
                    candidateName,
                    eventData.ElectionCategoryName,
                    eventData.ElectionName,
                    eventData.OrganizationName
                );

                await unitOfWork.AuditLogs.LogAsync(
                    eventData.AddedByUserId,
                    AuditAction.Create,
                    $"Candidate '{candidateName}' added to ElectionCategory '{eventData.ElectionCategoryName}' in election '{eventData.ElectionName}'"
                );
                await unitOfWork.SaveChangesAsync();

                Channel.BasicAck(args.DeliveryTag, false);
            };

            Channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }
    }
}