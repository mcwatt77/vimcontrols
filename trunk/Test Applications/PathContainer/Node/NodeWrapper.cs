using System.Linq;
using PathContainer.NodeImplementor;

namespace PathContainer.Node
{
    public class NodeWrapper : INode
    {
        private readonly Namespace _nameSpace;
        private readonly INodeImplementor _implementor;
        private readonly RootNode _root;

        public NodeWrapper(INode parent, RootNode root, Namespace nameSpace, INodeImplementor implementor)
        {
            Parent = parent;
            _root = root;
            _nameSpace = nameSpace;
            _implementor = implementor;
        }

        public string Name
        {
            get { return _implementor.Name; }
        }

        public Namespace NameSpace
        {
            get { return _nameSpace.Combine(_implementor.Namespace); }
        }

        public INode Parent { get; private set; }

        private INode WrapChild(INodeImplementor implementor)
        {
            return new NodeWrapper(this, _root, _nameSpace, implementor);
        }


        public INode Root
        {
            get
            {
                return _root;
            }
        }

        private INodeCollection _nodes;

        public INodeCollection Nodes
        {
            get
            {
                if (_nodes == null)
                    _nodes = new NodeWrapperCollection(_implementor.Nodes.Select(node => WrapChild(node)).ToArray());
                return _nodes;
            }
        }

        private IDataNodeCollection _dataNodes;

        public IDataNodeCollection DataNodes
        {
            get
            {
                if (_dataNodes == null)
                    _dataNodes = _root.WrapDataNodeCollection(this, _implementor.DataNodes);
                return _dataNodes;
            }
        }
    }
}