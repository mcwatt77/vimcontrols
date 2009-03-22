using System;
using ActionDictionary;
using ActionDictionary.Interfaces;
using NUnit.Framework;

namespace DataProcessors.Tests
{
    [TestFixture]
    public class ProxyGeneratorTest : IMissing
    {
        private TestImplementation _test;

        [Test]
        public void Test()
        {
            var i = 0;
            _test = new TestImplementation(@param => i = @param);
            var proxy = ProxyGenerator.Generate<TestInterface>(this);
//            var proxy = new TestDynamicImplementation(this);
            proxy.DoSomething(3);
            Assert.AreEqual(5, i);
        }

        public void ProcessMissingCmd(Message msg)
        {
            msg.Invoke(_test);
        }
    }

    public class Pgtest : TestInterface
    {
        private IMissing _missing;
        private readonly Action<int> _doSomething;

        public Pgtest(IMissing missing, object[] methods)
        {
            _missing = missing;
            _doSomething = (Action<int>)methods[0];
        }

        public void DoSomething(int i)
        {
            _doSomething(i);
        }
    }

    public interface TestInterface
    {
        void DoSomething(int i);
    }

    public class TestImplementation : TestInterface
    {
        private readonly Action<int> _fnDo;

        public TestImplementation(Action<int> fnDo)
        {
            _fnDo = fnDo;
        }

        public void DoSomething(int i)
        {
            _fnDo(i + 2);
        }
    }
}
