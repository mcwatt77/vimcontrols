using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Navigator.Repository
{
    public class RepositorySerializer
    {
        private readonly IdentityLookup _identityLookup;
        private int _numberOfSerializedObjects;

        public RepositorySerializer(IdentityLookup identityLookup)
        {
            _identityLookup = identityLookup;
        }

        public void SerializeObjects(IEnumerable<object> objects)
        {
            //TODO: The first section should be the type dictionary
            //Then the index to the type sections
            //Everything after that is just a list of type sections

            var fileStream = new FileStream("repository.dat", FileMode.Create);
            fileStream.Seek(0, SeekOrigin.End);

            Serializer.Serialize(fileStream, 0);

            var numberOfSerializedObjects = 0;

            var repositorySerializer = new RepositorySerializer(_identityLookup);
            foreach (var @object in objects)
                numberOfSerializedObjects += repositorySerializer.Serialize(fileStream, @object);

            fileStream.Seek(0, SeekOrigin.Begin);
            Serializer.Serialize(fileStream, numberOfSerializedObjects);

            fileStream.Flush();
            fileStream.Close();
        }

        public IEnumerable<TType> DeserializeByType<TType>()
        {
            var fileStream = new FileStream("repository.dat", FileMode.Open);

            var buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, buffer.Length);
            fileStream.Seek(0, SeekOrigin.Begin);

            var numberOfSerializedObjects = Serializer.DeserializeInt(fileStream);

            var repositorySerializer = new RepositorySerializer(_identityLookup);
            var objectEntries = Enumerable
                .Repeat(repositorySerializer, numberOfSerializedObjects)
                .Select(rs => Deserialize(fileStream))
                .ToArray();

            var lookupDictionary = objectEntries.ToDictionary(entry => entry.ObjectId, entry => entry.Object);

            var loadedObjects = new List<TType>();

            foreach (var entry in objectEntries)
            {
                entry.UpdateRecord(lookupDictionary);
                if (typeof(TType).IsAssignableFrom(entry.Object.GetType()))
                    loadedObjects.Add((TType)entry.Object);
            }
            return loadedObjects;
        }

        private int Serialize(Stream stream, object @object)
        {
            _numberOfSerializedObjects++;

            var type = @object.GetType();
            var properties = type.GetProperties();

            var serializers = properties
                .Select(propertyInfo => new
                                            {
                                                Property = propertyInfo,
                                                Serialize = GetSerializeMethod(propertyInfo.PropertyType),
                                                Value = propertyInfo.GetValue(@object, null)
                                            })
                .Where(a => a.Value != null)
                .Select(a => new
                                 {
                                     a.Property,
                                     a.Serialize,
                                     a.Value,
                                     Id = a.Serialize != null ? -1 : _identityLookup.LookupId(a.Value)
                                 })
                .ToArray();

            var valuesToPreSerialize = serializers
                .Where(a => a.Serialize == null)
                .ToArray();

            foreach (var a in valuesToPreSerialize)
                Serialize(stream, a.Value);

            var id = _identityLookup.LookupId(@object);
            Serializer.Serialize(stream, id);

            Serializer.Serialize(stream, type);

            Serializer.Serialize(stream, serializers.Length);

            foreach (var a in serializers)
                SerializeProperty(stream, a.Property, a.Serialize == null ? a.Id : a.Value);

            return _numberOfSerializedObjects;
        }

        public class ObjectEntry
        {
            public int ObjectId { get; set; }
            public object Object { get; set; }
            public List<KeyValuePair<PropertyInfo, object>> Values { get; set; }

            public void UpdateRecord(Dictionary<int, object> objectLookups)
            {
                foreach (var keyValuePair in Values)
                    UpdateField(keyValuePair, objectLookups);
            }

            private void UpdateField(KeyValuePair<PropertyInfo, object> pair, IDictionary<int, object> dictionary)
            {
                if (pair.Key.PropertyType == pair.Value.GetType())
                    pair.Key.SetValue(Object, pair.Value, null);
                else
                    pair.Key.SetValue(Object, dictionary[(int) pair.Value], null);
            }
        }

        private static ObjectEntry Deserialize(Stream stream)
        {
            var objectId = Serializer.DeserializeInt(stream);

            var objectType = Serializer.DeserializeType(stream);
            //deserialize properties

            var listOfProperty = new List<KeyValuePair<PropertyInfo, object>>();
            var numberOfProperties = Serializer.DeserializeInt(stream);
            foreach (var i in Enumerable.Range(0, numberOfProperties))
                listOfProperty.Add(DeserializeProperty(stream, objectType));

            var @object = Activator.CreateInstance(objectType);

            return new ObjectEntry
                       {
                           Object = @object,
                           ObjectId = objectId,
                           Values = listOfProperty
                       };
        }

        private static KeyValuePair<PropertyInfo, object> DeserializeProperty(Stream stream, Type parentType)
        {
            var propertyName = Serializer.DeserializeString(stream);

            var propertyInfo = parentType.GetProperty(propertyName);
            var method = GetDeserializeMethod(propertyInfo.PropertyType);

            return method != null
                       ? new KeyValuePair<PropertyInfo, object>(propertyInfo, method(stream))
                       : new KeyValuePair<PropertyInfo, object>(propertyInfo, Serializer.DeserializeInt(stream));
        }

        private static Func<Stream, object> GetDeserializeMethod(Type type)
        {
            var deserializeMethod = typeof (Serializer)
                .GetMethods()
                .Where(methodInfo => methodInfo.GetParameters().Count() == 1)
                .SingleOrDefault(methodInfo => methodInfo.ReturnType == type);

            if (deserializeMethod == null) return null;

            return stream => deserializeMethod.Invoke(null, new [] {stream});
        }

        private static Action<Stream, object> GetSerializeMethod(Type type)
        {
            var serializeMethod = typeof (Serializer)
                .GetMethods()
                .Where(methodInfo => methodInfo.GetParameters().Count() == 2)
                .SingleOrDefault(methodInfo => methodInfo.GetParameters().Skip(1).First().ParameterType == type);

            if (serializeMethod == null) return null;

            return (stream, @object) => serializeMethod.Invoke(null, new [] {stream, @object});
        }

        private static void SerializeProperty(Stream stream, PropertyInfo propertyInfo, object value)
        {
            var serializeMethod = typeof (Serializer)
                .GetMethods()
                .Where(methodInfo => methodInfo.GetParameters().Count() == 2)
                .SingleOrDefault(methodInfo => methodInfo.GetParameters().Skip(1).First().ParameterType == value.GetType());

            Serializer.Serialize(stream, propertyInfo.Name);

            serializeMethod.Invoke(null, new[] {stream, value});
        }
    }
}