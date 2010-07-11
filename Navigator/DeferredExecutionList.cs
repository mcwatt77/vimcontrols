using System;
using System.Collections;
using System.Collections.Generic;

namespace Navigator
{
    public class DeferredExecutionList<T> : IEnumerable<T>
    {
        private readonly Func<IEnumerable<T>> _getItems;
        private IEnumerable<T> _items;

        public DeferredExecutionList(Func<IEnumerable<T>> getItems)
        {
            _getItems = getItems;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_items == null)
                _items = _getItems();
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}