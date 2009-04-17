using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PathContainer.Node
{
    public class DataNodeCollectionInjector : IDataNodeCollection
    {
        private readonly NodeWrapper _wrapper;
        private readonly IEnumerable<Func<string, INode, IDataNode, IDataNode>> _delegates;
        private readonly IDataNodeCollection _nodes;

        public DataNodeCollectionInjector(NodeWrapper wrapper, IEnumerable<Func<string, INode, IDataNode, IDataNode>> delegates, IDataNodeCollection nodes)
        {
            _wrapper = wrapper;
            _delegates = delegates;
            _nodes = nodes;
        }

        public IEnumerator<KeyValuePair<string, IDataNode>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IDataNode GetBySlotName(string slotName)
        {
            if (_delegates.Count() == 1)
                return _delegates.First()(slotName, _wrapper, _nodes.GetBySlotName(slotName));
            return null;
        }

        public IDataNode Register(string slotName, Func<object> fnNew)
        {
            throw new System.NotImplementedException();
        }
    }
}