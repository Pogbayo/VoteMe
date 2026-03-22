using Hangfire;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.Infrastructure.Jobs
{
    public class ElectionScheduler : IElectionScheduler
    {
        //public void ScheduleOpenElection(Guid electionId, DateTimeOffset startDate)
        //{
        //    BackgroundJob.Schedule<IElectionJobService>(
        //        job => job.OpenElectionAsync(electionId),
        //        startDate
        //    );
        //}

        public void ScheduleCloseElection(Guid electionId, DateTimeOffset endDate)
        {
            BackgroundJob.Schedule<IElectionJobService>(
                job => job.CloseElectionAsync(electionId),
                endDate
            );
        }
    }
}
