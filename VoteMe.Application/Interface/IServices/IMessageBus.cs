namespace VoteMe.Application.Interface.IServices
{
    public interface IMessageBus
    {
        Task PublishAsync<T>(string queue, T message);
    }
}
