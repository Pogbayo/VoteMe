using VoteMe.Application.DTOs.ElectionCategory;

namespace VoteMe.Application.Helpers
{
    public static class EmailTemplates
    {
        public static (string Subject, string Body) WelcomeEmail(string fullName)
        {
            return (
                Subject: "Welcome to VoteMe 🎉",
                Body: $@"
                    <h2>Welcome to VoteMe, {fullName}!</h2>
                    <p>We're excited to have you on board.</p>
                    <p>You can now join organizations and participate in voting sessions.</p>
                    <br/>
                    <p>The VoteMe Team</p>
                "
            );
        }

        public static (string Subject, string Body) WelcomeToOrganizationEmail(string fullName, string organizationName)
        {
            return (
                Subject: $"You've joined {organizationName}",
                Body: $@"
                    <h2>Welcome, {fullName}!</h2>
                    <p>You have successfully joined <strong>{organizationName}</strong>.</p>
                    <p>You can now participate in all active elections within this organization.</p>
                    <br/>
                    <p>The VoteMe Team</p>
                "
            );
        }

        public static (string Subject, string Body) OtpEmail(string fullName, string otp)
        {
            return (
                Subject: "Your VoteMe OTP Code",
                Body: $@"
                    <h2>Hi {fullName},</h2>
                    <p>Your one-time password is:</p>
                    <h1 style='color: #4CAF50; letter-spacing: 5px;'>{otp}</h1>
                    <p>This code expires in <strong>10 minutes</strong>.</p>
                    <p>If you did not request this, please ignore this email.</p>
                    <br/>
                    <p>The VoteMe Team</p>
                "
            );
        }

        public static (string Subject, string Body) ElectionOpenedEmail(string electionName, string organizationName)
        {
            return (
                Subject: $"Election '{electionName}' is now open!",
                Body: $@"
            <h2>Attention!</h2>
            <p>The election <strong>{electionName}</strong> in <strong>{organizationName}</strong> is now open.</p>
            <p>Log in to VoteMe to cast your vote before it closes.</p>
            <br/>
            <p>The VoteMe Team</p>
        "
            );
        }

        public static (string Subject, string Body) ElectionClosedEmail(string electionName, string organizationName)
        {
            return (
                Subject: $"Election '{electionName}' has closed",
                Body: $@"
            <h2>Attention!</h2>
            <p>The election <strong>{electionName}</strong> in <strong>{organizationName}</strong> has now closed.</p>
            <p>Log in to VoteMe to view the final results.</p>
            <br/>
            <p>The VoteMe Team</p>
        "
            );
        }

        public static (string Subject, string Body) VoteConfirmationEmail(string fullName, string electionName, string candidateName)
        {
            return (
                Subject: $"Vote Confirmation - {electionName}",
                Body: $@"
                    <h2>Hi {fullName},</h2>
                    <p>Your vote has been successfully cast in <strong>{electionName}</strong>.</p>
                    <p>Thank you for participating in this election.</p>
                    <br/>
                    <p>The VoteMe Team</p>
                "
            );
        }

        public static (string Subject, string Body) VoteChangedEmail(string fullName, string electionName, string newCandidateName)
        {
            return (
                Subject: $"Vote Changed - {electionName}",
                Body: $@"
                    <h2>Hi {fullName},</h2>
                    <p>Your vote in <strong>{electionName}</strong> has been successfully updated.</p>
                    <p>If you did not make this change, please contact support immediately.</p>
                    <br/>
                    <p>The VoteMe Team</p>
                "
            );
        }

        public static (string Subject, string Body) PasswordChangedEmail(string fullName)
        {
            return (
                Subject: "Your VoteMe Password Has Been Changed",
                Body: $@"
                    <h2>Hi {fullName},</h2>
                    <p>Your password has been successfully changed.</p>
                    <p>If you did not make this change, please contact support immediately.</p>
                    <br/>
                    <p>The VoteMe Team</p>
                "
            );
        }

        public static (string Subject, string Body) ElectionResultsEmail(
           string electionName,
           List<ElectionCategoryResultDto> categoryResults,
           int totalVotes)
        {
            var resultsTable = string.Join("", categoryResults.Select(c =>
            {
                var winnerName = c.Winner?.DisplayName
                    ?? $"{c.Winner?.FirstName} {c.Winner?.LastName}"
                    ?? "No winner";

                return $@"
            <tr>
                <td style='padding:8px;border:1px solid #ddd'>{c.ElectionCategoryName}</td>
                <td style='padding:8px;border:1px solid #ddd'>{winnerName}</td>
                <td style='padding:8px;border:1px solid #ddd'>{c.TotalVotes}</td>
            </tr>";
            }));

            return (
                Subject: $"Results - {electionName}",
                Body: $@"
            <h2>Election Results</h2>
            <p>The results for <strong>{electionName}</strong> are in!</p>
            <p>Total votes cast: <strong>{totalVotes}</strong></p>
            <h3>Winners by ElectionCategory:</h3>
            <table style='border-collapse:collapse;width:100%'>
                <thead>
                    <tr>
                        <th style='padding:8px;border:1px solid #ddd;background:#f4f4f4'>ElectionCategory</th>
                        <th style='padding:8px;border:1px solid #ddd;background:#f4f4f4'>Winner</th>
                        <th style='padding:8px;border:1px solid #ddd;background:#f4f4f4'>Votes</th>
                    </tr>
                </thead>
                <tbody>
                    {resultsTable}
                </tbody>
            </table>
            <p>Thank you for participating.</p>
            <br/>
            <p>The VoteMe Team</p>
        "
            );
        }

        public static (string Subject, string Body) ElectionCreatedEmail(
            string electionName,
            string organizationName,
            List<string> categoryNames)
        {
            var categoriesList = string.Join("", categoryNames.Select(c => $"<li>{c}</li>"));

            return (
                Subject: $"New Election - {electionName}",
                Body: $@"
            <h2>New Election Created</h2>
            <p>A new election <strong>{electionName}</strong> has been created in <strong>{organizationName}</strong>.</p>
            <p>The following categories are available:</p>
            <ul>
                {categoriesList}
            </ul>
            <p>Voting will begin soon. Stay tuned!</p>
            <br/>
            <p>The VoteMe Team</p>
        "
            );
        }

        public static (string Subject, string Body) OrganizationCreatedEmail(string fullName, string organizationName, string uniqueKey)
        {
            return (
                Subject: $"Your Organization '{organizationName}' Has Been Created",
                Body: $@"
                    <h2>Hi {fullName},</h2>
                    <p>Your organization <strong>{organizationName}</strong> has been successfully created on VoteMe.</p>
                    <p>Share this unique key with your members so they can join:</p>
                    <h1 style='color: #4CAF50; letter-spacing: 5px;'>{uniqueKey}</h1>
                    <p>Keep this key safe.</p>
                    <br/>
                    <p>The VoteMe Team</p>
                "
            );
        }

        public static (string Subject, string Body) CandidateAddedEmail(
            string candidateName,
            string electionCategoryName,
            string electionName,
            string organizationName)
        {
            return (
                Subject: $"New Candidate Added - {electionName}",
                Body: $@"
            <h2>New Candidate Added</h2>
            <p>A new candidate <strong>{candidateName}</strong> has been added to the 
            <strong>{electionCategoryName}</strong> category in election <strong>{electionName}</strong> 
            at <strong>{organizationName}</strong>.</p>
            <p>Log in to VoteMe to view the updated candidate list.</p>
            <br/>
            <p>The VoteMe Team</p>
        "
            );
        }

        public static (string Subject, string Body) CandidateDeletedEmail(
            string candidateName,
            string electionCategoryName,
            string electionName,
            string organizationName)
        {
            return (
                Subject: $"Candidate Removed - {electionName}",
                Body: $@"
            <h2>Candidate Removed</h2>
            <p>The candidate <strong>{candidateName}</strong> has been removed from the 
            <strong>{electionCategoryName}</strong> category in election <strong>{electionName}</strong> 
            at <strong>{organizationName}</strong>.</p>
            <p>Log in to VoteMe to view the updated candidate list.</p>
            <br/>
            <p>The VoteMe Team</p>
        "
            );
        }
    }
}