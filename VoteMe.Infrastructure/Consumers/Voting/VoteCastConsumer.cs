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
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<Hub>>();

                await cacheService.RemoveAsync($"election-results-{eventData.ElectionId}");

                await notificationService.SendVoteConfirmationEmailAsync(
                    new List<string> { eventData.VoterEmail },
                    eventData.VoterDisplayName,
                    eventData.ElectionName,
                    eventData.CandidateFirstName
                );

                await unitOfWork.AuditLogs.LogAsync(
                     eventData.VoterId ?? Guid.Empty,
                    Domain.Enum.AuditAction.Votecast,
                     $"User voted for '{eventData.CandidateFirstName} {eventData.CandidateLastName}' in election '{eventData.ElectionName}'"
                 );
                await unitOfWork.SaveChangesAsync();

                var fullName = eventData.CandidateFirstName + "" + eventData.CandidateLastName;

                await hubContext.Clients.All.SendAsync("VoteCasted", new
                {
                    ElectionId = eventData.ElectionId,
                    ElectionCategoryId = eventData.ElectionCategoryId,
                    CandidateId = eventData.CandidateId,
                    ElectionCategoryName = eventData.ElectionCategoryName,
                    CandidateName = $"{eventData.CandidateFirstName} {eventData.CandidateLastName}",
                    VoterId = eventData.IsPrivate ? null : (Guid?)eventData.VoterId,
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