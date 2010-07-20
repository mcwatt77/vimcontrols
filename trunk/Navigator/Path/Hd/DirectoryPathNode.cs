using System.Collections.Generic;
using System.IO;
using System.Linq;
using Navigator.Containers;
using Navigator.UI.Attributes;

namespace Navigator.Path.Hd
{
    public class DirectoryPathNode : ISummaryString, IModelChildren
    {
        private readonly object _parent;
        private readonly IContainer _container;
        private readonly DirectoryInfo _directoryInfo;
        private object[] _children;

        public DirectoryPathNode(IContainer container, DirectoryInfo directoryInfo)
        {
            _container = container;
            _directoryInfo = directoryInfo;
        }

        public DirectoryPathNode(IContainer container, object parent, DirectoryInfo directoryInfo) : this(container, directoryInfo)
        {
            _parent = parent;
        }

        public IEnumerable<object> Children
        {
            get
            {
                if (_children != null) return _children;

                _children = (_parent != null ? new[] {_parent} : new object[]{})
                    .Concat(_directoryInfo.GetDirectories().Select(directory => (object) _container.Get<DirectoryPathNode>(this, directory)))
                    .Concat(_directoryInfo.GetFiles().Select(file => (object) _container.Get<FilePathNode>(file)))
                    .ToArray();
                return _children;
            }
        }

        public string Summary
        {
            get { return "[" + _directoryInfo.Name + "]"; }
        }
    }
}