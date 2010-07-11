using System;
using Navigator.UI.Attributes;

namespace Navigator.Path.Jobs
{
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
}