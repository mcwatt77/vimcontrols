using System;
using System.Linq;
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

        public override void AddToDictionary(MessageDictionary dictionary)
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

        private static Message GetModeChangeMessage(string eVal)
        {
            return Message.Create<IModeChange>(f => f.ChangeMode(GetMode(eVal)));
        }

        private static InputMode GetMode(string eVal)
        {
            return (InputMode)Enum.Parse(typeof(InputMode), eVal);
        }
    }
}