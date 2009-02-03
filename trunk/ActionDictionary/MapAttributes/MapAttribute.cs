using System;

namespace ActionDictionary.MapAttributes
{
    public abstract class MapAttribute : Attribute
    {
        public abstract void AddToDictionary(MessageDictionary dictionary);
    }
}