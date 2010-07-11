namespace Navigator.Path.Jobs
{
    public class RunCmd : Job
    {
        private readonly CommandInfo _info;

        public RunCmd(CommandInfo info)
        {
            _info = info;
        }

        public override string Summary
        {
            get { return "Run Command: " + _info.CommandName; }
        }

        public override void Execute()
        {
        }
    }
}