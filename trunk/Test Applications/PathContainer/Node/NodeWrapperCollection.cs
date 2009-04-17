using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PathContainer.Node
{
    public class NodeWrapperCollection : INodeCollection
    {
        private readonly IEnumerable<INode> _nodes;

        public NodeWrapperCollection(IEnumerable<INode> nodes)
        {
            _nodes = nodes;
        }

        public INode GetByName(string name)
        {
            return _nodes.SingleOrDefault(node => node.Name == name);
        }

        public IEnumerator<INode> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public INode FindById(string id)
        {
            return _nodes.SingleOrDefault(node => node.Nodes.GetByName("id").DataNodes.GetBySlotName(Slot.StringData).Cast<string>() == id);
        }

        public INode GetByName(string name, string nameSpace)
        {
            return _nodes.SingleOrDefault(node => node.Name == name && node.NameSpace.ToString() == nameSpace);
        }
    }
}