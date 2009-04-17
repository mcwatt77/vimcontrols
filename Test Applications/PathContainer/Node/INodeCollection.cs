using System.Collections.Generic;
using PathContainer.Node;

namespace PathContainer.Node
{
    public interface INodeCollection<TNode> : IEnumerable<TNode>
    {
        TNode GetByName(string name);
    }

    public interface INodeCollection : INodeCollection<INode>
    {
        INode FindById(string id);
        INode GetByName(string name, string nameSpace);
    }
}