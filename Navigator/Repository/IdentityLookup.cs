using System.Collections.Generic;

namespace Navigator.Repository
{
    public class IdentityLookup
    {
        private int _currentId = 0;

        private Dictionary<object, int> _idDictionary = new Dictionary<object, int>();

        public int GetNewId(object @object)
        {
            _idDictionary[@object] = _currentId;
            var oldId = _currentId;
            _currentId++;
            return oldId;
        }

        public int LookupId(object @object)
        {
            return _idDictionary[@object];
        }
    }
}