using System.Collections.Generic;

namespace NodeMessaging
{
    public interface IParentNode : INode
    {
        IEnumerable<IParentNode> Nodes(string nameFilter);
        IParentNode NodeAt(int index);
        IEndNode Attribute(string name);
        void Register<T>(T t);
    }
}