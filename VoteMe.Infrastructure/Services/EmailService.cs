using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IAmazonSimpleEmailService _sesClient;

        public EmailService(IAmazonSimpleEmailService sesClient)
        {
            _sesClient = sesClient;
        }

        public async Task<bool> SendEmailAsync(List<string> emailRecipients, string subject, string body)
        {

            var fromEmail = "noreply@tech-expert-beta.com.ng";
            var displayName = "VoteMe";

            var formattedSource = $"{displayName} <{fromEmail}>";

            var sendRequest = new SendEmailRequest
            {
                Source = formattedSource,
                Destination = new Destination
                {
                    ToAddresses = emailRecipients
                },

                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = new Body
                    {
                        Html = new Content(body)
                    }
                }
            };

            try
            {
                var response = await _sesClient.SendEmailAsync(sendRequest);
                Console.WriteLine($"Email sent! Message ID: {response.MessageId}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}
