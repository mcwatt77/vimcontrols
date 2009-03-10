using System;
using System.Linq.Expressions;
using ActionDictionary;
using ActionDictionary.Interfaces;
using AppControlInterfaces.NoteViewer;
using DataProcessors.NoteViewer;
using NUnit.Framework;
using Utility.Core;

namespace DataProcessors.Tests
{
    [TestFixture]
    public class TextTest : TestBase, ITextViewUpdate
    {
        [Test]
        public void TestSomething()
        {
            Test(new TextController(new TextCursor()) {Updater = this, TextProvider = new TextProvider("")},
                 controller => controller.TextProvider.Lines.SeparateBy("\r\n"));
        }

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
   
        private static Message _ti(Expression<Action<ITextInput>> fn)
        {
            return _m(fn);
        }

        public void UpdateTextRows()
        {
        }

        public void UpdateCursor()
        {
        }
    }
}
