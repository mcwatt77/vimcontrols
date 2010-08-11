using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Navigator.Repository
{
    public static class Serializer
    {
        public static void Serialize(Stream stream, Type type)
        {
            Serialize(stream, type.Assembly.Location);
            Serialize(stream, type.Namespace ?? "");
            Serialize(stream, type.Name);
        }

        public static Type DeserializeType(Stream stream)
        {
            var assemblyLocation = DeserializeString(stream);
            var @namespace = DeserializeString(stream);
            var name = DeserializeString(stream);

            var assembly = Assembly.LoadFile(assemblyLocation);
            return assembly.GetType(name);
        }

        public static void Serialize(Stream stream, long i)
        {
            var u = (ulong) i;
            Serialize(stream, (byte)(u >> 56));
            Serialize(stream, (byte)((u >> 48) & 0xFF));
            Serialize(stream, (byte)((u >> 40) & 0xFF));
            Serialize(stream, (byte)((u >> 32) & 0xFF));
            Serialize(stream, (byte)((u >> 24) & 0xFF));
            Serialize(stream, (byte)((u >> 16) & 0xFF));
            Serialize(stream, (byte)((u >> 8) & 0xFF));
            Serialize(stream, (byte)(u & 0xFF));
        }

        public static long DeserializeLong(Stream stream)
        {
            ulong i = 0;
            for (var s = 0; s <= 56; s += 8)
            {
                i <<= 8;
                i += DeserializeByte(stream);
            }

            return (long)i;
        }

        public static void Serialize(Stream stream, int i)
        {
            Serialize(stream, (byte)(i & 0xFF));
            Serialize(stream, (byte)((i >> 8) & 0xFF));
            Serialize(stream, (byte)((i >> 16) & 0xFF));
            Serialize(stream, (byte)(i >> 24));
        }

        public static int DeserializeInt(Stream stream)
        {
            int i = 0;
            i += DeserializeByte(stream);
            i += DeserializeByte(stream) << 8;
            i += DeserializeByte(stream) << 16;
            i += DeserializeByte(stream) << 24;

            return i;
        }

        public static void Serialize(Stream stream, byte b)
        {
            stream.WriteByte(b);
        }

        public static byte DeserializeByte(Stream stream)
        {
            return (byte) stream.ReadByte();
        }

        public static void Serialize(Stream stream, DateTime dateTime)
        {
            Serialize(stream, (byte) dateTime.Month);
            Serialize(stream, (byte) dateTime.Day);
            Serialize(stream, dateTime.Year);
            Serialize(stream, (byte) dateTime.Hour);
            Serialize(stream, (byte) dateTime.Minute);
            Serialize(stream, (byte) dateTime.Second);
            Serialize(stream, dateTime.Millisecond);
        }

        public static DateTime DeserializeDateTime(Stream stream)
        {
            var month = DeserializeByte(stream);
            var day = DeserializeByte(stream);
            var year = DeserializeInt(stream);
            var hour = DeserializeByte(stream);
            var minute = DeserializeByte(stream);
            var second = DeserializeByte(stream);
            var millisecond = DeserializeInt(stream);

            return new DateTime(year, month, day, hour, minute, second, millisecond);
        }

        public static void Serialize(Stream stream, string s)
        {
            var buffer = Encoding.ASCII.GetBytes(s);
            Serialize(stream, buffer.Length);
            stream.Write(buffer, 0, buffer.Length);
        }

        public static string DeserializeString(Stream stream)
        {
            var length = DeserializeInt(stream);

            var buffer = new byte[length];
            stream.Read(buffer, 0, length);

            return Encoding.ASCII.GetString(buffer);
        }
    }
}