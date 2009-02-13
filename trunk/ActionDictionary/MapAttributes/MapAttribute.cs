using System;
using System.Reflection;
using Utility.Core;

namespace ActionDictionary.MapAttributes
{
    public abstract class MapAttribute : Attribute
    {
        public abstract void AddToDictionary(MessageDictionary dictionary, MethodInfo info);

        protected static Message BuildMessage(MethodInfo info, params object[] @params)
        {
            return Message.Create(info.BuildLambda(@params), info.DeclaringType);
        }
    }
}