namespace VoteMe.Application.Interface.IServices
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(List<string> emailRecipients, string subject, string body);
    }
}
