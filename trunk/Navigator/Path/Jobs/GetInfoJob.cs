using Navigator.Containers;

namespace Navigator.Path.Jobs
{
    public class GetInfoJob : Job
    {
        private readonly JobProgress _jobProgress;

        public GetInfoJob(IContainer container)
        {
            _jobProgress = container.Get<JobProgress>();
        }

        public override string Summary
        {
            get { return "Get Info"; }
        }

        public override void Execute()
        {
            _jobProgress.StartJob();
        }
    }
}