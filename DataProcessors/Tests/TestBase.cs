using System;
using ActionDictionary;

namespace DataProcessors.Tests
{
    public abstract class TestBase
    {
        protected abstract void SetKeyStrings();

        protected void AddKeyString(string s, object expect, params Message[] messages)
        {}

        protected Message _m<T>(Action<T> fn)
        {
            return null;
        }
    }
}
