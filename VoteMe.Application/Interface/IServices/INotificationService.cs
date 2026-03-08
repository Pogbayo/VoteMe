namespace VoteMe.Application.Interface.IServices
{
    public interface INotificationService
    {
        Task SendWelcomeEmailAsync(List<string> emails, string fullName);
        Task SendOtpEmailAsync(List<string> emails, string fullName, string otp);
        Task SendElectionOpenedEmailAsync(List<string> emails, string electionTitle, string organizationName);
        Task SendElectionClosedEmailAsync(List<string> emails, string electionTitle, string organizationName);
        Task SendVoteConfirmationEmailAsync(List<string> emails, string fullName, string electionTitle, string candidateName);
        Task SendVoteChangedEmailAsync(List<string> emails, string fullName, string electionTitle, string newCandidateName);
        Task SendPasswordChangedEmailAsync(List<string> emails, string fullName);
        Task SendElectionResultsEmailAsync(List<string> emails, string electionTitle, string winnerName, int totalVotes);
        Task SendOrganizationCreatedEmailAsync(List<string> emails, string fullName, string organizationName, string uniqueKey);
        Task SendWelcomeToOrganizationEmailAsync(List<string> emails, string fullName, string organizationName);
    }
}