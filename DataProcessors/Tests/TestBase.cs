using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ActionDictionary;
using KeyStringParser;
using NUnit.Framework;
using Utility.Core;

namespace DataProcessors.Tests
{
    public abstract class TestBase
    {
        private readonly List<KeyStringData> _keyStrings = new List<KeyStringData>();

        protected void Test<TAction>(Func<TAction> fnMsgRecipient, Func<TAction, string> fn)
        {
            SetKeyStrings();
            _keyStrings.Do(keyString => keyString.Test(fnMsgRecipient(), fn));
        }

        protected abstract void SetKeyStrings();

        protected void AddKeyString(string s, string expect, params Message[] messages)
        {
            _keyStrings.Add(new KeyStringData {Input = s, Output = expect, Messages = messages});
        }

        protected static Message _m<T>(Expression<Action<T>> fn)
        {
            return Message.Create(fn);
        }

        private class KeyStringData
        {
            private static readonly MessageDictionary _mDict = new MessageDictionary();

            public string Input { get; set; }
            public string Output { get; set; }
            public Message[] Messages { get; set; }

            public void Test<TAction>(TAction msgRecipient, Func<TAction, string> fn)
            {
                var parser = new Parser();
                var parse = parser.Parse(Input).ToList();
                var msgs = parse.Select(key => _mDict.ProcessKey(key)).Flatten();
                msgs.Do(msg => msg.Invoke(msgRecipient));
                Assert.AreEqual(Output, fn(msgRecipient));
            }
        }
    }
}
