using System.Collections.Generic;
using System.Linq;
using Utility.Core;

namespace NodeMessaging
{
    public class AggregateNode : IParentNode
    {
        private readonly IParentNode[] _nodes;

        public AggregateNode(params IParentNode[] nodes)
        {
            _nodes = nodes;
        }

        public string Name
        {
            get { throw new System.NotImplementedException(); }
        }

        public IParentNode Parent
        {
            get { throw new System.NotImplementedException(); }
        }

        public IEnumerable<IParentNode> Nodes(string nameFilter)
        {
            return _nodes.Select(node => node.Nodes(nameFilter)).Flatten();
        }

        public IParentNode NodeAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public IEndNode Attribute(string name)
        {
            throw new System.NotImplementedException();
        }

        public T Get<T>() where T : class
        {
            throw new System.NotImplementedException();
        }

        public void Register<T>(T t)
        {
            throw new System.NotImplementedException();
        }
    }
}