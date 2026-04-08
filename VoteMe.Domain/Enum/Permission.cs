namespace VoteMe.Domain.Enum
{
    public enum Permission
    {
        // Organization
        CreateOrganization,
        UpdateOrganization,
        DeleteOrganization,
        ViewOrganization,

        // Election
        CreateElection,
        UpdateElection,
        DeleteElection,
        OpenElection,
        CloseElection,
        ViewElectionResults,

        // Election Category
        CreateElectionCategory,
        UpdateElectionCategory,
        DeleteElectionCategory,

        // Candidate
        CreateCandidate,
        UpdateCandidate,
        DeleteCandidate,

        // Voting
        Vote,
        ViewLiveResults,

        // Member Management
        InviteMember,
        ApproveMember,
        RemoveMember,
        PromoteToAdmin,
        DemoteFromAdmin,
        ViewMembers
    }
}