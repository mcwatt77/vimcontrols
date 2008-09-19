using System.Collections;
using System.Collections.Generic;

namespace VIMControls
{
    public class LinkedList<T> : ILinkedList<T>
    {
        private readonly System.Collections.Generic.LinkedList<T> _data = new System.Collections.Generic.LinkedList<T>();

        public IEnumerator<T> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ILinkedListNode<T> AddFirst(T item)
        {
            return new LinkedListNode<T>(_data.AddFirst(item));
        }

        public void AddBefore(ILinkedListNode<T> node, T item)
        {
            _data.AddBefore(((LinkedListNode<T>) node).Node, item);
        }
    }

    public class LinkedListNode<T> : ILinkedListNode<T>
    {
        public LinkedListNode(System.Collections.Generic.LinkedListNode<T> node)
        {
            Node = node;
        }

        public System.Collections.Generic.LinkedListNode<T> Node { get; set; }
    }

    public interface ILinkedList<T> : ILinkedListNode<T>, IEnumerable<T>
    {
        ILinkedListNode<T> AddFirst(T item);
        void AddBefore(ILinkedListNode<T> node, T item);
    }

    public interface ILinkedListNode<T>
    {
    }
}