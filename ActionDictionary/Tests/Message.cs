using NUnit.Framework;

namespace ActionDictionary.Tests
{
    [TestFixture]
    public class MessageTest
    {
        [Test]
        public void Test()
        {
            var msg = Message.Create<ITestInterface>(i => i.DoSomething(2));
            var test = new TestImplementation();
            msg.Invoke(test);

            Assert.AreEqual(2, test.Value);
        }
    }

    public class TestImplementation : ITestInterface
    {
        public int Value { get; set; }

        public void DoSomething(int i)
        {
            Value += i;
        }
    }

    public interface ITestInterface
    {
        void DoSomething(int i);
    }
}
