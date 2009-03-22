using System.Collections.Generic;

namespace NodeMessaging
{
    public class ParentNodeWrapper : IParentNode
    {
        private readonly RootNode _rootNode;
        private readonly IParentNode _node;

        public ParentNodeWrapper(RootNode rootNode, IParentNode node)
        {
            _rootNode = rootNode;
            _node = node;
        }

        public string Name
        {
            get { throw new System.NotImplementedException(); }
        }

        public IEnumerable<IParentNode> Nodes(string nameFilter)
        {
            throw new System.NotImplementedException();
        }

        public IParentNode NodeAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public IEndNode Attribute(string name)
        {
            return new EndNodeWrapper(_rootNode, _node.Attribute(name));
        }

        public IParentNode Parent
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}