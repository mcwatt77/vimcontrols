using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Navigator.UI.Attributes;

namespace Navigator.Path.Hd
{
    public class HdPath : ISummaryString, IModelChildren
    {
        private readonly IEnumerable<object> _children;

        public HdPath()
        {
            _children = Environment.GetLogicalDrives()
                .Select(driveName => (object)new DirectoryPathNode(this, new DirectoryInfo(driveName)))
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