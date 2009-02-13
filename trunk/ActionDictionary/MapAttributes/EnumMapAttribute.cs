using System;
using System.Linq;
using System.Reflection;
using Utility.Core;

namespace ActionDictionary.MapAttributes
{
    internal class EnumMapAttribute : MapAttribute
    {
        private readonly Type _type;
        private readonly string[] _mappings;

        public EnumMapAttribute(Type type, params string[] mappings)
        {
            _type = type;
            _mappings = mappings;
        }

        public override void AddToDictionary(MessageDictionary dictionary, MethodInfo info)
        {
            _mappings.Do(mapping => AddMapping(dictionary, mapping, info));
        }

        private void AddMapping(MessageDictionary dictionary, string mapping, MethodInfo info)
        {
            var elems = mapping.Split(' ');
            var mode = GetMode(elems.First());

            var mapCount = (elems.Count() - 1)/2;
            Enumerable.Range(0, mapCount).Do(i =>
                                                 {
                                                     var mappings = elems.Skip(i*2 + 1).Take(2);
                                                     dictionary.AddString(mode, mappings.First(), GetModeChangeMessage(mappings.Skip(1).First(), info));
                                                 });
        }

        private Message GetModeChangeMessage(string eVal, MethodInfo info)
        {
            return Message.Create(info.BuildLambda(GetEnum(eVal, _type)), info.DeclaringType);
        }

        private static InputMode GetMode(string eVal)
        {
            return (InputMode)Enum.Parse(typeof(InputMode), eVal);
        }

        private static object GetEnum(string eVal, Type type)
        {
            return Enum.Parse(type, eVal);
        }
    }
}