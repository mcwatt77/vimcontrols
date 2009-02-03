using System.Windows.Input;
using ActionDictionary.MapAttributes;
using KeyStringParser;

namespace ActionDictionary.Interfaces
{
    public interface ITextInput
    {
        [CharacterMap(InputMode.Insert, "<a-z><space><1-0>", "abcdefghijklmnopqrstuvwxyz 1234567890")]
        [CharacterMapShift(InputMode.Insert, "<A-Z>!@#$%^&*()~_+{}:|", "ABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()~_+{}:|")]
        [CharacterMapKeys(InputMode.Insert, " ", 2, Key.LeftShift, Key.Space)]
        void InputCharacter(char c);
    }

    internal class CharacterMapKeysAttribute : MapAttribute
    {
        public CharacterMapKeysAttribute(InputMode mode, string charList, int width, params Key[] keys)
        {}

        public override void AddToDictionary(MessageDictionary dictionary)
        {
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
//            _parser.Parse(_list).Do(key => dictionary.AddKey(_mode, key, ));
        }
    }
}
