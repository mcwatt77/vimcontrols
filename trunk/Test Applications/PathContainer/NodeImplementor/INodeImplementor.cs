using PathContainer.Node;

namespace PathContainer.NodeImplementor
{
    public interface INodeImplementor
    {
        string Name { get; set; }
        Namespace Namespace { get; set; }
        INodeCollection<INodeImplementor> Nodes { get; }
        IDataNodeCollection DataNodes { get; }
    }
}