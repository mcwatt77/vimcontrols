using System.Collections.Generic;

namespace NodeMessaging
{
    public interface IParentNode : INode
    {
        IEnumerable<IParentNode> Nodes(string nameFilter);
        IEnumerable<IParentNode> Nodes(string nameSpace, string nameFilter);
        IEnumerable<IParentNode> Nodes();
        IParentNode NodeAt(int index);
        IEndNode Attribute(string name);
        IEnumerable<IEndNode> Attributes();
        IParentNode NodeById(string id);
    }
}