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
}