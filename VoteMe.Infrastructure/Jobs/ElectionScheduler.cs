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

        public void ScheduleCloseElection(Guid electionId, DateTime endDate)
        {
            BackgroundJob.Schedule<ElectionJobService>(
                job => job.CloseElectionAsync(electionId),
                endDate
            );
        }
    }
}
