using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using ActionDictionary.Interfaces;
using ActionDictionary.MapAttributes;
using KeyStringParser;
using Utility.Core;

namespace ActionDictionary
{
    public class MessageDictionary : IModeChange
    {
        // Suppress warning.  These are only accessed through reflection
#pragma warning disable 169
        private readonly SequencedDictionary<Key, Message> _normalDict = new SequencedDictionary<Key, Message>();
        private readonly SequencedDictionary<Key, Message> _insertDict = new SequencedDictionary<Key, Message>();
#pragma warning restore 169
        private SequencedDictionary<Key, Message> _currentDict;
        private static readonly Parser _parser = new Parser();

        public MessageDictionary()
        {
            ReadCommandsFromAttributes();
            AddString(InputMode.Insert, "a", Message.Create<ITextInput>(f => f.InputCharacter('a')));

            _currentDict = _normalDict;
            InputMode = InputMode.Normal;
        }

        private void ReadCommandsFromAttributes()
        {
            typeof (MessageDictionary)
                .Assembly
                .GetTypes()
                .Select(type => type.GetMethods().AsEnumerable())
                .Flatten()
                .Select(type => type.AttributesOfType<MapAttribute>().SingleOrDefault())
                .Where(map => map != null)
                .Do(map => map.AddToDictionary(this));
        }

        public InputMode InputMode { get; private set; }

        public void AddString(InputMode mode, string keys, Message msg)
        {
            ChangeMode(mode);
            _currentDict.Add(_parser.Parse(keys), msg);
        }

        public void AddKey(InputMode mode, Key key, Message msg)
        {
            ChangeMode(mode);
            _currentDict.Add(new []{key}, msg);
        }

        public IEnumerable<Message> ProcessKey(Key key)
        {
            var ret = new List<Message>();
            _currentDict.Push(key);
            while (_currentDict.Count() > 0)
                ret.Add(_currentDict.Pop().Value);
            ret.Do(msg => msg.Invoke(this));
            return ret;
        }

        public void ChangeMode(InputMode mode)
        {
            var member = GetType().GetField(string.Format("_{0}Dict", mode.ToString().ToLower()), BindingFlags.NonPublic | BindingFlags.Instance);
            _currentDict = (SequencedDictionary<Key, Message>)member.GetValue(this);
            InputMode = mode;
        }
    }
}