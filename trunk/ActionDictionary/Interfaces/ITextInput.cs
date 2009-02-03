using System;
using System.Linq;
using System.Windows.Input;
using ActionDictionary.MapAttributes;
using KeyStringParser;
using Utility.Core;

namespace ActionDictionary.Interfaces
{
    public interface ITextInput
    {
        [CharacterMap(InputMode.Insert, "<a-z><space>1234567890", "abcdefghijklmnopqrstuvwxyz 1234567890")]
        [CharacterMapShift(InputMode.Insert, "ABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()~_+{}:|", "ABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()~_+{}:|")]
        [CharacterMapKeys(InputMode.Insert, " ", 2, Key.LeftShift, Key.Space)]
        void InputCharacter(char c);
    }

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

        public override void AddToDictionary(MessageDictionary dictionary)
        {
            if (_keys.Length != _charList.Length * _width)
                throw new Exception("CharacterMapAttribute requires the key list and the character list to be of proportional length");

            Enumerable.Range(0, _charList.Length).Do(
                i =>
                dictionary.AddKeys(_mode, _keys.Skip(i*2).Take(2),
                                   Message.Create<ITextInput>(f => f.InputCharacter(_charList[i]))));
        }
    }

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

        public override void AddToDictionary(MessageDictionary dictionary)
        {
            var keyList = _parser.Parse(_keyList);
            if (keyList.Count() != _charList.Length * 2)
                throw new Exception("CharacterMapAttribute requires the key list and the character list to be of identical length");

            Enumerable.Range(0, _charList.Length).Do(
                i =>
                dictionary.AddKeys(_mode, keyList.Skip(i*2).Take(2),
                                   Message.Create<ITextInput>(f => f.InputCharacter(_charList[i]))));
        }
    }
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

        public override void AddToDictionary(MessageDictionary dictionary)
        {
            var keyList = _parser.Parse(_keyList);
            if (keyList.Count() != _charList.Length)
                throw new Exception("CharacterMapAttribute requires the key list and the character list to be of identical length");

            keyList.Do(
                (i, key) =>
                dictionary.AddKey(_mode, key, Message.Create<ITextInput>(f => f.InputCharacter(_charList[i]))));
        }
    }
}
