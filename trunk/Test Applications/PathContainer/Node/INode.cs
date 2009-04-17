using PathContainer.Node;
using PathContainer.NodeImplementor;

namespace PathContainer.Node
{
    public interface INode
    {
        string Name { get; }
        Namespace NameSpace { get; }

        INode Parent { get; }
        INode Root { get; }
        INodeCollection Nodes { get; }
        IDataNodeCollection DataNodes { get; }
    }
}