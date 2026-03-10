

namespace VoteMe.Application.Interface.IServices
{
    public interface IElectionJobService
    {
        Task OpenElectionAsync(Guid electionId);
        Task CloseElectionAsync(Guid electionId);
    }
}
