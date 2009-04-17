using System;
using System.Collections;
using System.Collections.Generic;

namespace PathContainer.Node
{
    public class DataNodeWrapperCollection : IDataNodeCollection
    {
        private readonly IEnumerable<IDataNode> _dataNodes;

        public DataNodeWrapperCollection(IEnumerable<IDataNode> dataNodes)
        {
            _dataNodes = dataNodes;
        }

        public IEnumerator<System.Collections.Generic.KeyValuePair<string, IDataNode>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IDataNode GetBySlotName(string slotName)
        {
            throw new System.NotImplementedException();
        }

        public IDataNode Register(string slotName, Func<object> fnNew)
        {
            throw new System.NotImplementedException();
        }
    }
}