using System.Linq;
using NUnit.Framework;

namespace KeyStringParser
{
    [TestFixture]
    public class SequencedDictionaryTest
    {
        private void Pop(SequencedDictionary<char, string> dict)
        {
            Enumerable
                .Range(0, dict.Count())
                .Select(i => dict.Pop())
                .ToList();
        }

        [Test]
        public void TestFlush()
        {}

        [Test]
        public void Test()
        {
            var dict = new SequencedDictionary<char, string>();
            dict.Add("sam", "sam");
            dict.Add("sa", "sa");
            dict.Add("zebra", "zebra");
            dict.Add("ze", "ze");
            dict.Add("charlie", "charlie");
            dict.Add("te", "te");

            dict.Push('s');
            Assert.AreEqual(0, dict.Count());
            dict.Push('a');
            Assert.AreEqual(0, dict.Count());
            dict.Push('m');
            Assert.AreEqual(1, dict.Count());
            Assert.AreEqual("sam", dict.Pop().Value);

            dict.Push('z');
            dict.Push('e');
            dict.Push('b');
            dict.Push('d');
            Assert.AreEqual("ze", dict.Pop().Value);

            var dictPop = dict.Pop();
            Assert.AreEqual('b', dictPop.Keys.Single());
            Assert.IsNull(dictPop.Value);

            dictPop = dict.Pop();
            Assert.AreEqual('d', dictPop.Keys.Single());
            Assert.IsNull(dictPop.Value);

            dict.Push('c');
            Assert.AreEqual(0, dict.Count());
            dict.Push('h');
            Assert.AreEqual(0, dict.Count());
            dict.Push('d');
            Assert.AreEqual(3, dict.Count());

            dictPop = dict.Pop();
            Assert.AreEqual('c', dictPop.Keys.Single());
            Assert.IsNull(dictPop.Value);

            dictPop = dict.Pop();
            Assert.AreEqual('h', dictPop.Keys.Single());
            Assert.IsNull(dictPop.Value);

            dictPop = dict.Pop();
            Assert.AreEqual('d', dictPop.Keys.Single());
            Assert.IsNull(dictPop.Value);

            dict.Push('t');
            dict.Push('e');
            Assert.AreEqual(1, dict.Count());
            Assert.AreEqual("te", dict.Pop().Value);


/*            var dict = new SequencedDictionary<char, string>();
            dict.Add("sam", "sam");
            dict.Add("s", "s");

            SequencedDictionary<char, string>.ResultToken token = null;
            token = dict.Get('s', token);
            token = dict.Get('a', token);
            token = dict.Get('p', token);
            Assert.IsFalse(token.IsComplete);
            Assert.AreEqual(1, token.Values.Count());
            Assert.AreEqual("s", token.Values.First());

            token = null;
            token = dict.Get('s', token);
            token = dict.Get('a', token);
            token = dict.Get('m', token);
            Assert.IsTrue(token.IsComplete);
            Assert.AreEqual(1, token.Values.Count());
            Assert.AreEqual("sam", token.Values.First());*/
        }
    }
}
