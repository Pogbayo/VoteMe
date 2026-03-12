using VoteMe.Application.DTOs.ElectionCategory;

namespace VoteMe.Application.Interface.IServices
{
    public interface INotificationService
    {
        Task SendWelcomeEmailAsync(List<string> emails, string fullName);
        Task SendOtpEmailAsync(List<string> emails, string fullName, string otp);
        Task SendElectionOpenedEmailAsync(List<string> emails, string electionName, string organizationName, List<string> categoryNames);
        Task SendElectionClosedEmailAsync(List<string> emails, string electionName, string organizationName);
        Task SendVoteConfirmationEmailAsync(List<string> emails, string fullName, string electionName, string candidateName);
        Task SendVoteChangedEmailAsync(List<string> emails, string fullName, string electionName, string newCandidateName);
        Task<bool> SendElectionCreatedEmailAsync(List<string> emails, string electionName, string organizationName, List<string> categoryNames);
        Task SendMemberLeftOrganizationEmailAsync(List<string> emails, string displayName, string organizationName);
        Task SendPasswordChangedEmailAsync(List<string> emails, string fullName);
        Task<bool> SendCandidateAddedEmailAsync(List<string> emails, string candidateName, string electionCategoryName, string electionName, string organizationName);
        Task<bool> SendCandidateDeletedEmailAsync(List<string> emails, string candidateName, string electionCategoryName, string electionName, string organizationName);
        Task<bool> SendElectionResultsEmailAsync(List<string> emails, string electionName, List<ElectionCategoryResultDto> categoryResults, int totalVotes); Task SendOrganizationCreatedEmailAsync(List<string> emails, string fullName, string organizationName, string uniqueKey);
        Task SendWelcomeToOrganizationEmailAsync(List<string> emails, string fullName, string organizationName);
        Task SendOrganizationDeletedNotificationAsync(string organizationName,List<string> memberEmails,DateTime deletedAt);
    }
}