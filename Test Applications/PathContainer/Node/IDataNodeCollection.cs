using System;
using System.Collections.Generic;
using PathContainer.Node;

namespace PathContainer.Node
{
    public interface IDataNodeCollection : IEnumerable<KeyValuePair<string, IDataNode>>
    {
        IDataNode GetBySlotName(string slotName);
        IDataNode Register(string slotName, Func<object> fnNew);
    }
}