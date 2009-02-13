using System;
using System.Linq;
using System.Reflection;
using ActionDictionary.MapAttributes;
using KeyStringParser;
using Utility.Core;

namespace ActionDictionary.MapAttributes
{
    internal class CharacterMapShiftAttribute : MapAttribute
    {
        private readonly InputMode _mode;
        private readonly string _keyList;
        private readonly string _charList;
        private static readonly Parser _parser = new Parser();

        public CharacterMapShiftAttribute(InputMode mode, string keyList, string charList)
        {
            _mode = mode;
            _keyList = keyList;
            _charList = charList;
        }

        public override void AddToDictionary(MessageDictionary dictionary, MethodInfo info)
        {
            var keyList = _parser.Parse(_keyList);
            if (keyList.Count() != _charList.Length * 2)
                throw new Exception("CharacterMapAttribute requires the key list and the character list to be of identical length");

            Enumerable.Range(0, _charList.Length).Do(
                i =>
                dictionary.AddKeys(_mode, keyList.Skip(i*2).Take(2),
                                   BuildMessage(info, _charList[i])));
        }
    }
}