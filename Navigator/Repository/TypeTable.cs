using System;
using System.Collections.Generic;
using System.IO;

namespace Navigator.Repository
{
    public class TypeTable : IHasSerializer<TypeTable>
    {
        private readonly Dictionary<Type, int> _typeIds = new Dictionary<Type, int>();

        public void Serialize(Stream stream)
        {
            Serializer.Serialize(stream, _typeIds.Count);

            foreach (var keyValuePair in _typeIds)
            {
                Serializer.Serialize(stream, keyValuePair.Key);
                Serializer.Serialize(stream, keyValuePair.Value);
            }
        }

        public int GetTypeId(Type type)
        {
            if (!_typeIds.ContainsKey(type))
                _typeIds[type] = _typeIds.Count;
            return _typeIds[type];
        }

        public TypeTable Deserialize(Stream stream)
        {
            var typeTable = new TypeTable();
            var itemCount = Serializer.DeserializeInt(stream);

            for (var i = 0; i < itemCount; )
            {
                var type = Serializer.DeserializeType(stream);
                var id = Serializer.DeserializeInt(stream);
                _typeIds[type] = id;
            }

            return typeTable;
        }
    }
}