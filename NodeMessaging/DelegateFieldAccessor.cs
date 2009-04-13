using System;

namespace NodeMessaging
{
    public class DelegateFieldAccessor<T> : IAccessor<T>
    {
        private readonly Func<T> _get;
        private readonly Action<T> _set;

        public DelegateFieldAccessor(Func<T> get, Action<T> set)
        {
            _get = get;
            _set = set;
        }

        public T Value
        {
            get { return _get(); }
            set { _set(value); }
        }
    }
}