using System;
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
    public class MessageDictionary : IModeChange, IAliasMap
    {
        // Suppress warning.  These are only accessed through reflection
#pragma warning disable 169
        private readonly SequencedDictionary<Key, Message> _normalDict = new SequencedDictionary<Key, Message>();
        private readonly SequencedDictionary<Key, Message> _insertDict = new SequencedDictionary<Key, Message>();
        private readonly SequencedDictionary<Key, Message> _commandDict = new SequencedDictionary<Key, Message>();
#pragma warning restore 169
        private SequencedDictionary<Key, Message> _currentDict;
        private static readonly Parser _parser = new Parser();

        public MessageDictionary()
        {
            ReadCommandsFromAttributes();
//            AddString(InputMode.Insert, "a", Message.Create<ITextInput>(f => f.InputCharacter('a')));

            _currentDict = _normalDict;
            InputMode = InputMode.Normal;
        }

/*        private void ReadCommandsFromAttributes()
        {
            typeof (MessageDictionary)
                .Assembly
                .GetTypes()
                .Select(type => type.GetMethods().AsEnumerable())
                .Flatten()
                .Select(type => type.AttributesOfType<MapAttribute>().AsEnumerable())
                .Flatten()
                .Where(map => map != null)
                .Do(map => map.AddToDictionary(this, null));
        }*/
        private void ReadCommandsFromAttributes()
        {
            typeof (MessageDictionary)
                .Assembly
                .GetTypes()
                .Select(type => type.GetMethods().AsEnumerable())
                .Flatten()
                .Where(method => method.AttributesOfType<MapAttribute>().Count() > 0)
                .Select(type => new {Attribute = type.AttributesOfType<MapAttribute>(), Method = type})
                .Do(map => map.Attribute.Do(attr => attr.AddToDictionary(this, map.Method)));
        }

        public InputMode InputMode { get; private set; }

        public void AddString(InputMode mode, string keys, Message msg)
        {
            ChangeMode(mode);
            _currentDict.Add(_parser.Parse(keys), msg);
        }

        public void AddKeys(InputMode mode, IEnumerable<Key> keys, Message msg)
        {
            ChangeMode(mode);
            _currentDict.Add(keys, msg);
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
            {
                while (_currentDict.Count() > 0)
                {
                    //don't do this if Value == null
                    var value = _currentDict.Pop().Value;
                    if (value == null) continue;
                    ret.Add(value);
                }
                try
                {
                    ret.Do(msg => msg.Invoke(this));
                }
                catch (TargetInvocationException e)
                {
                    throw e.InnerException;
                }
            }
            return ret;
        }

        public void ChangeMode(InputMode mode)
        {
            var member = GetType().GetField(string.Format("_{0}Dict", mode.ToString().ToLower()), BindingFlags.NonPublic | BindingFlags.Instance);
            if (member == null) throw new Exception(mode + " does not have a dictionary set up");
            _currentDict = (SequencedDictionary<Key, Message>)member.GetValue(this);
            InputMode = mode;
        }

        public void SetAlias(Key keyAlias)
        {
            _currentDict.Push(keyAlias);
        }
    }
}