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
//                                container.GetOrDefault<GetInfoJob>(),
                                container.GetOrDefault<RunCmd>(new CommandInfo
                                                                   (@"D:\development\casemax\cmsdotnet\framework\trunk\database")
                                                                   {
                                                                       Command = @"nant -buildfile:database.build full-upgrade",
                                                                       CommandName = "Build trunk database"
                                                                   }),
                                container.GetOrDefault<RunCmd>(new CommandInfo
                                                                   (@"D:\development\casemax\cmsdotnet\framework\trunk")
                                                                   {
                                                                       Command = @"config.bat",
                                                                       CommandName = "Switch to trunk configuration"
                                                                   }),
                                container.GetOrDefault<RunCmd>(new CommandInfo
                                                                   (@"D:\development\casemax\cmsdotnet\framework\trunk\PRS.CMS")
                                                                   {
                                                                       Command = @"nant build test",
                                                                       CommandName = "Test trunk"
                                                                   }),
                                container.GetOrDefault<RunCmd>(new CommandInfo
                                                                   (@"D:\development\casemax\cmsdotnet\framework\branches\10.2.0\database")
                                                                   {
                                                                       Command = @"nant -buildfile:database.build full-upgrade",
                                                                       CommandName = "Build 10.2.0 database"
                                                                   }),
                                container.GetOrDefault<RunCmd>(new CommandInfo
                                                                   (@"D:\development\casemax\cmsdotnet\framework\branches\10.2.0")
                                                                   {
                                                                       Command = @"config.bat",
                                                                       CommandName = "Switch to 10.2.0 configuration"
                                                                   }),
                                container.GetOrDefault<RunCmd>(new CommandInfo
                                                                   (@"D:\development\casemax\cmsdotnet\framework\branches\10.2.0\PRS.CMS")
                                                                   {
                                                                       Command = @"nant build test",
                                                                       CommandName = "Test 10.2.0"
                                                                   })
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