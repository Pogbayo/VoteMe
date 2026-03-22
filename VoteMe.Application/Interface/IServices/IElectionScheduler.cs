namespace VoteMe.Application.Interface.IServices
{
    public interface IElectionScheduler
    {
        //void ScheduleOpenElection(Guid electionId, DateTimeOffset startDate);
        void ScheduleCloseElection(Guid electionId, DateTimeOffset endDate);
    }
}
