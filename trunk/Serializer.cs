using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace VIMControls
{
    public class Serializer
    {
        private static readonly Dictionary<Type, Action<Stream, object>> _serializerDictionary =
            InitializeSerializerDictionary<SerializeFor, Action<Stream, object>>();

        private static readonly Dictionary<Type, Func<Stream, object>> _deserializerDictionary =
            InitializeSerializerDictionary<DeserializeFor, Func<Stream, object>>(
                method => s => method.Invoke(null, new object[] {s}));

        private static readonly Dictionary<Type, MethodInfo> _deserializerGenericDictionary =
            InitializeSerializerDictionary<DeserializeForGeneric, MethodInfo>(method => method);

        private static Dictionary<Type, TDelegateType> InitializeSerializerDictionary<TSerializeType, TDelegateType>() where TSerializeType : ISerializeAttribute
        {
            return InitializeSerializerDictionary<TSerializeType, TDelegateType>(
                method => method.CreateDelegate<TDelegateType>());
        }

        private static Dictionary<Type, TDelegateType> InitializeSerializerDictionary<TSerializeType, TDelegateType>(Func<MethodInfo, TDelegateType> fn)
            where TSerializeType : ISerializeAttribute
        {
            return typeof (Serializer)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .Where(method => method.AttributesOfType<TSerializeType>().Count() == 1)
                .ToDictionary(
                method => method.AttributesOfType<TSerializeType>().Single().Type,
                fn);
        }

        [SerializeFor(typeof(int))]
        private static void SerializeInt(Stream stream, object o)
        {
            var i = (int) o;
            stream.WriteByte((byte)(i&0xFF));
            stream.WriteByte((byte)((i >> 4)&0xFF));
            stream.WriteByte((byte)((i >> 8)&0xFF));
            stream.WriteByte((byte)((i >> 12)&0xFF));
        }

        [DeserializeFor(typeof(int))]
        private static int DeserializeInt(Stream stream)
        {
            var i = stream.ReadByte();
            i |= stream.ReadByte() << 4;
            i |= stream.ReadByte() << 8;
            i |= stream.ReadByte() << 12;
            return i;
        }

        [SerializeFor(typeof(string))]
        private static void SerializeString(Stream stream, object o)
        {
            var s = (string) o;
            SerializeInt(stream, s.Length);
            stream.Write(Encoding.ASCII.GetBytes(s), 0, s.Length);
        }

        [DeserializeFor(typeof(string))]
        private static string DeserializeString(Stream stream)
        {
            var length = DeserializeInt(stream);
            var buf = new byte[length];
            stream.Read(buf, 0, length);
            return Encoding.ASCII.GetString(buf);
        }

        [SerializeFor(typeof (KeyValuePair<,>))]
        private static void SerializeKeyValuePair(Stream stream, object o)
        {
            var keyProp = o.GetType().GetProperty("Key");
            var key = keyProp.GetValue(o, null);
            Serialize(stream, key);

            var valProp = o.GetType().GetProperty("Value");
            var val = valProp.GetValue(o, null);
            Serialize(stream, val);
        }

        [DeserializeForGeneric(typeof(KeyValuePair<,>))]
        private static KeyValuePair<TKey, TValue> DeserializeKeyValuePair<TKey, TValue>(Stream stream)
        {
            var method = typeof (Serializer).GetMethod("Deserialize").MakeGenericMethod(new [] {typeof (TKey)});
            var key = (TKey)method.Invoke(null, new object[] {stream});

            method = typeof (Serializer).GetMethod("Deserialize").MakeGenericMethod(new [] {typeof (TValue)});
            var val = (TValue) method.Invoke(null, new object[] {stream});

            return new KeyValuePair<TKey, TValue>(key, val);
        }

        [DeserializeForGeneric(typeof(List<>))]
        private static List<TItem> DeserializeList<TItem>(Stream stream)
        {
            var list = new List<TItem>();
            var i = DeserializeInt(stream);
            Enumerable.Range(0, i)
                .Do(i0 => list.Add(Deserialize<TItem>(stream)));
            return list;
        }

        [DeserializeForGeneric(typeof(Dictionary<,>))]
        private static Dictionary<TKey, TValue> DeserializeDictionary<TKey, TValue>(Stream stream)
        {
            var dict = new Dictionary<TKey, TValue>();
            var i = DeserializeInt(stream);
            Enumerable.Range(0, i)
                .Select(i0 => Deserialize<KeyValuePair<TKey, TValue>>(stream))
                .Do(keyValuePair => dict[keyValuePair.Key] = keyValuePair.Value);
            return dict;
        }

        public static void Serialize(Stream stream, object o)
        {
            var type = o.GetType();

            if (_serializerDictionary.ContainsKey(type))
            {
                _serializerDictionary[type](stream, o);
                return;
            }

            var e = o as IEnumerable;
            if (e != null)
            {
                var bufStream = new MemoryStream();
                var count = 0;
                e.Cast<object>().Do(item =>
                                        {
                                            Serialize(bufStream, item);
                                            count++;
                                        });
                bufStream.Position = 0;
//                SerializeString(stream, type.FullName);
                SerializeInt(stream, count);
                var buf = bufStream.GetBuffer();
                stream.Write(buf, 0, buf.Length);
                return;
            }

            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
                _serializerDictionary[genericType](stream, o);
            }

            //need to be careful with this, could hit recursion
            //have to do this differently, actually, and explicitly handle the fact that these could be
            //object references

//            SerializeClass(stream, o, type);
        }

        private static void SerializeClass(Stream stream, object o, Type type)
        {
            SerializeString(stream, type.FullName + ", " + type.Assembly.GetName().Name);

            type
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Do(property => Serialize(stream, property.GetValue(o, null)));

            type
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Do(field => Serialize(stream, field.GetValue(o)));
        }

        public static T Deserialize<T>(Stream stream)
        {
            var type = typeof (T);
            if (_deserializerDictionary.ContainsKey(type))
                return (T)_deserializerDictionary[type](stream);
            if (type.IsGenericType)
            {
                var method = _deserializerGenericDictionary[type.GetGenericTypeDefinition()];
                method = method.MakeGenericMethod(type.GetGenericArguments());
                return (T)method.Invoke(null, new object[]{stream});
            }

            throw new Exception("Failed to find a type deserializer.");
        }

        private interface ISerializeAttribute
        {
            Type Type { get; }
        }

        private class SerializeFor : Attribute, ISerializeAttribute
        {
            public SerializeFor(Type type)
            {
                Type = type;
            }

            public Type Type { get; private set; }
        }

        private class DeserializeForGeneric : Attribute, ISerializeAttribute
        {
            public DeserializeForGeneric(Type type)
            {
                Type = type;
            }

            public Type Type { get; private set; }
        }

        private class DeserializeFor : Attribute, ISerializeAttribute
        {
            public DeserializeFor(Type type)
            {
                Type = type;
            }

            public Type Type { get; private set; }
        }
    }
}