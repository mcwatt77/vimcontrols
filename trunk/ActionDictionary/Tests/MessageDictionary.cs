using System.Linq;
using System.Windows.Input;
using NUnit.Framework;

namespace ActionDictionary.Tests
{
    [TestFixture]
    public class MessageDictionaryTest
    {
        [Test]
        public void TestModeChange()
        {
            var msgDict = new MessageDictionary();
            msgDict.ChangeMode(InputMode.Normal);
        }

        [Test]
        public void TestAutoProcess()
        {
            var msgDict = new MessageDictionary();
            Assert.AreEqual(InputMode.Normal, msgDict.InputMode);
            msgDict.ProcessKey(Key.I);
            Assert.AreEqual(InputMode.Insert, msgDict.InputMode);
            msgDict.ProcessKey(Key.Escape);
            Assert.AreEqual(InputMode.Normal, msgDict.InputMode);
        }

        [Test]
        public void TestCommand()
        {
            var msgDict = new MessageDictionary();
            msgDict.ProcessKey(Key.LeftShift);
            msgDict.ProcessKey(Key.OemSemicolon);
        }

        [Test]
        public void TestSomeStuff()
        {
            var msgDict = new MessageDictionary();
            var msgs = msgDict.ProcessKey(Key.A);
            Assert.AreEqual(2, msgs.Count(), "Failed to enter insert mode");
            //A should send to Messages:  ChangeMode and InsertAfter
            msgDict.ProcessKey(Key.RightShift);
            msgs = msgDict.ProcessKey(Key.A);
            Assert.AreEqual("a => a.InputCharacter(A)", msgs.Single().ToString(), "Did not process RightShift");
        }
    }
}