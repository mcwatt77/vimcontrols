using System.Collections.Generic;

namespace NodeMessaging
{
    public interface IParentNodeImplementor : INodeImplementor
    {
        IEnumerable<IParentNodeImplementor> Nodes(string nameFilter);
        IEnumerable<IParentNodeImplementor> Nodes();
        IParentNodeImplementor NodeAt(int index);
        IEndNodeImplementor Attribute(string name);
        IEnumerable<IEndNodeImplementor> Attributes();
    }
}