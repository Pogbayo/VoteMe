namespace VoteMe.Application.Interface.IServices
{
    public interface IElectionScheduler
    {
        //void ScheduleOpenElection(Guid electionId, DateTime startDate);
        void ScheduleCloseElection(Guid electionId, DateTime endDate);
    }
}
