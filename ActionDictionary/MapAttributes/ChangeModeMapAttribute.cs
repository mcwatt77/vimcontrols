using System;
using System.Linq;
using System.Reflection;
using ActionDictionary.Interfaces;
using Utility.Core;

namespace ActionDictionary.MapAttributes
{
    internal class ChangeModeMapAttribute : MapAttribute
    {
        private readonly string[] _mappings;

        public ChangeModeMapAttribute(params string[] mappings)
        {
            _mappings = mappings;
        }

        public override void AddToDictionary(MessageDictionary dictionary, MethodInfo info)
        {
            _mappings.Do(mapping => AddMapping(dictionary, mapping));
        }

        private static void AddMapping(MessageDictionary dictionary, string mapping)
        {
            var elems = mapping.Split(' ');
            var mode = GetMode(elems.First());

            var mapCount = (elems.Count() - 1)/2;
            Enumerable.Range(0, mapCount).Do(i =>
                                                 {
                                                     var mappings = elems.Skip(i*2 + 1).Take(2);
                                                     dictionary.AddString(mode, mappings.First(), GetModeChangeMessage(mappings.Skip(1).First()));
                                                 });
        }

        //This is preferred because it allows pretty display
        private static Message GetModeChangeMessage(string eVal)
        {
            var mode = GetMode(eVal);
            var lambda = typeof (IModeChange).GetMethod("ChangeMode").BuildLambda(mode);
            return Message.Create(lambda, typeof(IModeChange));
        }

        private static InputMode GetMode(string eVal)
        {
            return (InputMode)Enum.Parse(typeof(InputMode), eVal);
        }
    }
}