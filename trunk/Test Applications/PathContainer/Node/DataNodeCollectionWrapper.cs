using System;
using System.Collections;
using System.Collections.Generic;

namespace PathContainer.Node
{
    public class DataNodeCollectionWrapper : IDataNodeCollection
    {
        private readonly IDataNodeCollection _dataNodeCollection;

        public DataNodeCollectionWrapper(IDataNodeCollection dataNodeCollection)
        {
            _dataNodeCollection = dataNodeCollection;
        }

        public IEnumerator<KeyValuePair<string, IDataNode>> GetEnumerator()
        {
            return _dataNodeCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IDataNode GetBySlotName(string slotName)
        {
            return _dataNodeCollection.GetBySlotName(slotName);
        }

        public IDataNode Register(string slotName, Func<object> fnNew)
        {
            return _dataNodeCollection.Register(slotName, fnNew);
        }
    }
}