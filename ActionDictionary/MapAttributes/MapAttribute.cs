using System;
using System.Reflection;

namespace ActionDictionary.MapAttributes
{
    public abstract class MapAttribute : Attribute
    {
        public abstract void AddToDictionary(MessageDictionary dictionary, MethodInfo info);
    }
}