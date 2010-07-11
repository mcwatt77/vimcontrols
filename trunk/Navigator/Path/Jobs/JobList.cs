using System;
using System.Collections.Generic;
using Navigator.Containers;
using Navigator.UI.Attributes;

namespace Navigator.Path.Jobs
{
    public class JobList : ISummaryString, IModelChildren
    {
        private readonly object[] _children;

        public JobList(IContainer container)
        {
            _children = new object[]
                            {
                                container.GetOrDefault<GetInfoJob>(),
                                container.GetOrDefault<RunCmd>(new CommandInfo {Command = "something.bat", CommandName = "Do Stuff"})
                            };
        }

        public string Summary
        {
            get { return "Jobs"; }
        }

        public IEnumerable<object> Children
        {
            get { return _children; }
        }
    }

    public abstract class Job : ISummaryString, IDescriptionString, INavigable
    {
        public abstract string Summary { get; }

        public abstract void Execute();

        public string Description
        {
            get { return "This is a job!"; }
        }

        public void Navigate()
        {
            //TODO: make this threaded.  Will have to give children a way to report progress back through some sort of interface,
            //that must ultimately originate from a container
            Execute();
        }

        public void NavigateToCurrentChild()
        {
            throw new NotImplementedException();
        }
    }

    public class JobProgress
    {}

    public class GetInfoJob : Job
    {
        public override string Summary
        {
            get { return "Get Info"; }
        }

        public override void Execute()
        {
        }
    }

    public class CommandInfo
    {
        public string Command { get; set; }
        public string CommandName { get; set; }
    }

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