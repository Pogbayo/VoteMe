namespace VoteMe.Domain.Enum
{
    public enum AuditAction
    {
        // Data operations
        Create,
        Update, 
        Delete,
        Read,

        // Authentication
        Login,
        Logout,
        FailedLogin,
        PasswordChanged,

        // Workflow
        Approve,
        Reject,
        Failed,
        Error,
        Transfer,

        ElectionClose,
        ElectionOpen,
        Join,
        Remove,
        Left,
        Votecast,
        VoteChanged,
        DemoteFromAdmin,
        PromoteToAdmin
    }
}


