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
    }
}