using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;

namespace NodeMessaging
{
    public class ParentNodeWrapper : NodeBase, IParentNode
    {
        private readonly RootNode _rootNode;
        private readonly IParentNode _node;
        private readonly Dictionary<string, IEnumerable<IParentNode>> _nodeDict = new Dictionary<string, IEnumerable<IParentNode>>();
        private IEnumerable<IEndNode> _attributes;

        public ParentNodeWrapper(RootNode rootNode, IParentNode node)
        {
            _rootNode = rootNode;
            _node = node;
        }

        public string Name
        {
            get { return _node.Name; }
        }

        public IEnumerable<IParentNode> Nodes(string nameFilter)
        {
            if (!_nodeDict.ContainsKey(nameFilter))
            {
                _nodeDict[nameFilter] = _node.Nodes(nameFilter).Select(node => (IParentNode)new ParentNodeWrapper(_rootNode, node)).ToList();
            }
            return _nodeDict[nameFilter];
        }

        public IEnumerable<IParentNode> Nodes()
        {
            if (!_nodeDict.ContainsKey(""))
                _nodeDict[""] = _node.Nodes().Select(node => (IParentNode) new ParentNodeWrapper(_rootNode, node)).ToList();
            return _nodeDict[""];
        }

        public IParentNode NodeAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public IEndNode Attribute(string name)
        {
            return Attributes().Single(attr => attr.Name == name);
        }

        public IEnumerable<IEndNode> Attributes()
        {
            if (_attributes == null)
                _attributes = _node.Attributes().Select(attr => (IEndNode)new EndNodeWrapper(_rootNode, attr)).ToList();
            return _attributes;
        }

        public T Get<T>() where T : class
        {
            var ret = (T) _registeredTypes[typeof (T)];
            return InjectNode(ret, Intercept);
        }

        //TODO: Remove duplication
        private void Intercept(IInvocation invocation)
        {
            if (invocation.TargetType != typeof(INode) && invocation.Method.DeclaringType == typeof(INode))
            {
                invocation.ReturnValue = invocation.Method.Invoke(_node, invocation.Arguments);
                return;
            }
            invocation.Proceed();
            _rootNode.Intercept(_node, invocation);
        }

        //TODO: Remove duplication
        private static T InjectNode<T>(T t, Action<IInvocation> fnIntercept) where T : class
        {
            var generator = new ProxyGenerator();
            var interfaces = t.GetType().GetInterfaces().Concat(new[] {typeof (IEndNode)}).ToArray();
            var proxy = (T) generator.CreateInterfaceProxyWithTarget(typeof(T), interfaces, t, new DelegateInterceptor(fnIntercept));
            //TODO: Include all child interfaces in Proxy
            return proxy;
        }

        public IParentNode Parent
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}