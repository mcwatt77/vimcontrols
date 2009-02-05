using System;
using System.Linq;
using System.Reflection;
using ActionDictionary.Interfaces;
using ActionDictionary.MapAttributes;
using KeyStringParser;
using Utility.Core;

namespace ActionDictionary.MapAttributes
{
    internal class CharacterMapAttribute : MapAttribute
    {
        private readonly InputMode _mode;
        private readonly string _keyList;
        private readonly string _charList;
        private static readonly Parser _parser = new Parser();

        public CharacterMapAttribute(InputMode mode, string keyList, string charList)
        {
            _mode = mode;
            _keyList = keyList;
            _charList = charList;
        }

        public override void AddToDictionary(MessageDictionary dictionary, MethodInfo info)
        {
            var keyList = _parser.Parse(_keyList);
            if (keyList.Count() != _charList.Length)
                throw new Exception("CharacterMapAttribute requires the key list and the character list to be of identical length");

            keyList.Do(
                (i, key) =>
                dictionary.AddKey(_mode, key, BuildMessage(_charList[i])));
        }

        private static Message BuildMessage(char i)
        {
            var lambda = typeof (ITextInput).GetMethod("InputCharacter").BuildLambda(i);
            return Message.Create(lambda, typeof(ITextInput));
        }
    }
}