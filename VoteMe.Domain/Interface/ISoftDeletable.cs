
namespace VoteMe.Domain.Interface
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}
