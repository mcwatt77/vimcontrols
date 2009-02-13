using System.Reflection;

namespace ActionDictionary.MapAttributes
{
    internal class KeyMapAttribute : MapAttribute
    {
        private readonly InputMode _mode;
        private readonly string _keyList;

        public KeyMapAttribute(InputMode mode, string keyList)
        {
            _mode = mode;
            _keyList = keyList;
        }

        public override void AddToDictionary(MessageDictionary dictionary, MethodInfo info)
        {
            dictionary.AddString(_mode, _keyList, BuildMessage(info));
        }
    }
}