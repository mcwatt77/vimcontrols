using System.Collections.Generic;
using System.Linq;

namespace NodeMessaging
{
    public class ParentNodeWrapper : NodeBase, IParentNode
    {
        private readonly IParentNode _contextRoot;
        private readonly Dictionary<string, IEnumerable<IParentNode>> _nodeDict = new Dictionary<string, IEnumerable<IParentNode>>();
        private IEnumerable<IEndNode> _attributes;
        private readonly IParentNodeImplementor _parentNode;
        private readonly IParentNode _parent;

        private ParentNodeWrapper(RootNode rootNode, IParentNode contextRoot, IParentNodeImplementor node, IParentNode parent) : base(rootNode, node)
        {
            _contextRoot = contextRoot;
            _parentNode = node;
            _parent = parent;
        }
            
        public ParentNodeWrapper(RootNode rootNode, IParentNodeImplementor node, IParentNode parent) : base(rootNode, node)
        {
            _contextRoot = this;
            _parentNode = node;
            _parent = parent;
        }

        public string Name
        {
            get { return _node.Name; }
        }

        public IEnumerable<IParentNode> Nodes(string nameFilter)
        {
            if (!_nodeDict.ContainsKey(nameFilter))
            {
                _nodeDict[nameFilter] = _parentNode.Nodes(nameFilter).Select(node => (IParentNode)new ParentNodeWrapper(_rootNode, _contextRoot, node, this)).ToList();
            }
            return _nodeDict[nameFilter];
        }

        public IEnumerable<IParentNode> Nodes(string nameSpace, string nameFilter)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IParentNode> Nodes()
        {
            if (!_nodeDict.ContainsKey(""))
                _nodeDict[""] = _parentNode.Nodes().Select(node => (IParentNode) new ParentNodeWrapper(_rootNode, _contextRoot, node, this)).ToList();
            return _nodeDict[""];
        }

        public IParentNode NodeAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public IEndNode Attribute(string name)
        {
            return Attributes().SingleOrDefault(attr => attr.Name == name);
        }

        public IEnumerable<IEndNode> Attributes()
        {
            if (_attributes == null)
                _attributes = _parentNode.Attributes().Select(attr => (IEndNode)new EndNodeWrapper(_rootNode, attr, this)).ToList();
            return _attributes;
        }

        public IParentNode NodeById(string id)
        {
            return _rootNode.NodeById(id);
        }

        public IParentNode Root
        {
            get { return _contextRoot; }
        }

        public IParentNode Parent
        {
            get { return _parent; }
        }
    }
}