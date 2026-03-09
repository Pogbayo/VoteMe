using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using VoteMe.Application.Events.Voting;
using VoteMe.Application.Interface.IRepositories;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.Infrastructure.Consumers.Voting
{
    public class VoteChangedConsumer : BaseConsumer
    {
        private readonly IServiceScopeFactory _scopeFactory;
        protected override string QueueName => "vote-changed";

        public VoteChangedConsumer(
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
                var eventData = JsonSerializer.Deserialize<VoteChangedEvent>(json);

                if (eventData == null) return;

                using var scope = _scopeFactory.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<Hub>>();

                await cacheService.RemoveAsync($"election-results-{eventData.ElectionId}");

                await notificationService.SendVoteChangedEmailAsync(
                     new List<string> { eventData.VoterEmail },
                     eventData.VoterDisplayName,
                     eventData.ElectionName,
                     $"{eventData.NewCandidateFirstName} {eventData.NewCandidateLastName}"
                 );

                await unitOfWork.AuditLogs.LogAsync(
                   eventData.UserId ?? Guid.Empty,
                   "VoteChanged",
                   "Vote",
                   $"User changed vote to '{eventData.NewCandidateFirstName} {eventData.NewCandidateLastName}' in election '{eventData.ElectionName}'"
               );
                await unitOfWork.SaveChangesAsync();

                await hubContext.Clients.All.SendAsync("VoteUpdated", new
                {
                    ElectionId = eventData.ElectionId,
                    ElectionCategoryId = eventData.ElectionCategoryId,
                    OldCandidateId = eventData.OldCandidateId,
                    NewCandidateId = eventData.NewCandidateId,
                    NewCandidateName = $"{eventData.NewCandidateFirstName} {eventData.NewCandidateLastName}",
                    VoterId = eventData.IsPrivate ? null : (Guid?)eventData.UserId,
                    VoterName = eventData.IsPrivate ? "Anonymous" : eventData.VoterDisplayName,
                    UpdatedAt = DateTime.UtcNow
                });

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