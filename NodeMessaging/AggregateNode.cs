using System.Collections.Generic;
using System.Linq;
using Utility.Core;

namespace NodeMessaging
{
    public class AggregateNode : IParentNodeImplementor
    {
        private readonly IParentNodeImplementor[] _nodes;

        public AggregateNode(params IParentNodeImplementor[] nodes)
        {
            _nodes = nodes;
        }

        public string Name
        {
            get { throw new System.NotImplementedException(); }
        }

        public IParentNodeImplementor Parent
        {
            get { throw new System.NotImplementedException(); }
        }

        public IEnumerable<IParentNodeImplementor> Nodes(string nameFilter)
        {
            return _nodes.Select(node => node.Nodes(nameFilter)).Flatten();
        }

        public IEnumerable<IParentNodeImplementor> Nodes()
        {
            return _nodes.Select(node => node.Nodes()).Flatten();
        }

        public IParentNodeImplementor NodeAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public IEndNodeImplementor Attribute(string name)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IEndNodeImplementor> Attributes()
        {
            throw new System.NotImplementedException();
        }
    }
}