using System;
using ActionDictionary;
using ActionDictionary.Interfaces;
using NUnit.Framework;

namespace DataProcessors.Tests
{
    [TestFixture]
    public class TextTest : TestBase
    {
        [Test]
        public void TestSomething()
        {}

        protected override void SetKeyStrings()
        {
            AddKeyString("aa", "a",
                         _ti(a => a.InsertAfterCursor()),
                         _ti(a => a.InputCharacter('a')));
            AddKeyString("a<cr><cr><cr><bk><bk><bk>", "",
                         _ti(a => a.InsertAfterCursor()),
                         _ti(a => a.InputCharacter('\n')),
                         _ti(a => a.InputCharacter('\n')),
                         _ti(a => a.InputCharacter('\n')));
        }
   
        private Message _ti(Action<ITextInput> fn)
        {
            return _m(fn);
        }
    }
}
