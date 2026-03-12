using VoteMe.Application.DTOs.ElectionCategory;
using VoteMe.Application.Helpers;
using VoteMe.Application.Interface.IServices;

public class NotificationService : INotificationService
{
    private readonly IEmailService _emailService;

    public NotificationService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task SendWelcomeEmailAsync(List<string> emails, string fullName)
    {
        var (subject, body) = EmailTemplates.WelcomeEmail(fullName);
        await _emailService.SendEmailAsync(emails, subject, body);
    }

    public async Task SendOtpEmailAsync(List<string> emails, string fullName, string otp)
    {
        var (subject, body) = EmailTemplates.OtpEmail(fullName, otp);
        await _emailService.SendEmailAsync(emails, subject, body);
    }

    public async Task SendElectionOpenedEmailAsync(List<string> emails, string electionName, string organizationName, List<string> categoryNames)
    {
        var (subject, body) = EmailTemplates.ElectionOpenedEmail(electionName, organizationName, categoryNames);
        await _emailService.SendEmailAsync(emails, subject, body);
    }

    public async Task SendElectionClosedEmailAsync(List<string> emails, string electionTitle, string organizationName)
    {
        var (subject, body) = EmailTemplates.ElectionClosedEmail(electionTitle, organizationName);
        await _emailService.SendEmailAsync(emails, subject, body);
    }

    public async Task SendVoteConfirmationEmailAsync(List<string> emails, string fullName, string electionTitle, string candidateName)
    {
        var (subject, body) = EmailTemplates.VoteConfirmationEmail(fullName, electionTitle, candidateName);
        await _emailService.SendEmailAsync(emails, subject, body);
    }

    public async Task SendVoteChangedEmailAsync(List<string> emails, string fullName, string electionTitle, string newCandidateName)
    {
        var (subject, body) = EmailTemplates.VoteChangedEmail(fullName, electionTitle, newCandidateName);
        await _emailService.SendEmailAsync(emails, subject, body);
    }

    public async Task SendPasswordChangedEmailAsync(List<string> emails, string fullName)
    {
        var (subject, body) = EmailTemplates.PasswordChangedEmail(fullName);
        await _emailService.SendEmailAsync(emails, subject, body);
    }

    public async Task SendOrganizationCreatedEmailAsync(List<string> emails, string fullName, string organizationName, string uniqueKey)
    {
        var (subject, body) = EmailTemplates.OrganizationCreatedEmail(fullName, organizationName, uniqueKey);
        await _emailService.SendEmailAsync(emails, subject, body);
    }

    public async Task SendWelcomeToOrganizationEmailAsync(List<string> emails, string fullName, string organizationName)
    {
        var (subject, body) = EmailTemplates.WelcomeToOrganizationEmail(fullName, organizationName);
        await _emailService.SendEmailAsync(emails, subject, body);
    }

    public async Task<bool> SendElectionResultsEmailAsync(List<string> emails, string electionName,List<ElectionCategoryResultDto> categoryResults,int totalVotes)
    {
        var (subject, body) = EmailTemplates.ElectionResultsEmail(electionName, categoryResults, totalVotes);
        return await _emailService.SendEmailAsync(emails, subject, body);
    }

    public async Task<bool> SendElectionCreatedEmailAsync( List<string> emails,string electionName,string organizationName,List<string> categoryNames)
    {
        var (subject, body) = EmailTemplates.ElectionCreatedEmail(electionName, organizationName, categoryNames);
        return await _emailService.SendEmailAsync(emails, subject, body);
    }

    public async Task<bool> SendCandidateAddedEmailAsync(
        List<string> emails,
        string candidateName,
        string electionCategoryName,
        string electionName,
        string organizationName)
    {
        var (subject, body) = EmailTemplates.CandidateAddedEmail(candidateName, electionCategoryName, electionName, organizationName);
        return await _emailService.SendEmailAsync(emails, subject, body);
    }

    public async Task<bool> SendCandidateDeletedEmailAsync(
        List<string> emails,
        string candidateName,
        string electionCategoryName,
        string electionName,
        string organizationName)
    {
        var (subject, body) = EmailTemplates.CandidateDeletedEmail(candidateName, electionCategoryName, electionName, organizationName);
        return await _emailService.SendEmailAsync(emails, subject, body);
    }

    public async Task SendMemberLeftOrganizationEmailAsync(List<string> emails, string displayName, string organizationName)
    {
        var (subject, body) = EmailTemplates.MemberLeftOrganizationEmail(displayName, organizationName);
        await _emailService.SendEmailAsync(emails, subject, body);
    }

    public async Task SendOrganizationDeletedNotificationAsync(
    string organizationName,
    List<string> memberEmails,
    DateTime deletedAt)
    {
        var (subject, body) = EmailTemplates.OrganizationDeletedEmail(
            organizationName,
            deletedAt);

        await _emailService.SendEmailAsync(
            memberEmails,
            subject,
            body);
    }
}