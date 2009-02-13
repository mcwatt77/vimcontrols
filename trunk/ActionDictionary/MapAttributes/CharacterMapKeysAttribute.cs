using System;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using ActionDictionary.MapAttributes;
using Utility.Core;

namespace ActionDictionary.MapAttributes
{
    internal class CharacterMapKeysAttribute : MapAttribute
    {
        private readonly InputMode _mode;
        private readonly string _charList;
        private readonly int _width;
        private readonly Key[] _keys;

        public CharacterMapKeysAttribute(InputMode mode, string charList, int width, params Key[] keys)
        {
            _mode = mode;
            _charList = charList;
            _width = width;
            _keys = keys;
        }

        public override void AddToDictionary(MessageDictionary dictionary, MethodInfo info)
        {
            if (_keys.Length != _charList.Length * _width)
                throw new Exception("CharacterMapAttribute requires the key list and the character list to be of proportional length");

            Enumerable.Range(0, _charList.Length).Do(
                i =>
                dictionary.AddKeys(_mode, _keys.Skip(i*2).Take(2),
                                   BuildMessage(info, _charList[i])));
        }
    }
}