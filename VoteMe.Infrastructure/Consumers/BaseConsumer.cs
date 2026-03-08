using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;

namespace VoteMe.Infrastructure.Consumers
{
    public abstract class BaseConsumer : BackgroundService, IDisposable
    {
        protected readonly IModel Channel;
        protected readonly IConnection Connection;
        protected abstract string QueueName { get; }

        protected BaseConsumer(IConfiguration configuration)
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:Host"],
                UserName = configuration["RabbitMQ:Username"],
                Password = configuration["RabbitMQ:Password"],
                VirtualHost = configuration["RabbitMQ:VirtualHost"]
            };

            Connection = factory.CreateConnection();
            Channel = Connection.CreateModel();
            Channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );
        }

        public override void Dispose()
        {
            Channel?.Dispose();
            Connection?.Dispose();
            base.Dispose();
        }
    }
}