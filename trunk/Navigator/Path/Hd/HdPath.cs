using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Navigator.Containers;
using Navigator.UI.Attributes;

namespace Navigator.Path.Hd
{
    public class HdPath : ISummaryString, IModelChildren
    {
        private readonly IEnumerable<object> _children;

        public HdPath(IContainer container)
        {
            _children = Environment.GetLogicalDrives()
                .Select(driveName => (object)container.Get<DirectoryPathNode>(this, new DirectoryInfo(driveName)))
                .ToArray();
        }

        public IEnumerable<object> Children
        {
            get
            {
                return _children;
            }
        }

        public string Summary
        {
            get { return "Computer"; }
        }
    }
}