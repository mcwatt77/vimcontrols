using ActionDictionary;
using ActionDictionary.Interfaces;

namespace DataProcessors.Tests
{
    public class TestDynamicImplementation : TestInterface
    {
        private readonly IMissing _missing;

        public TestDynamicImplementation(IMissing missing)
        {
            _missing = missing;
        }

        public void DoSomething(int i)
        {
            var msg = Message.Create<TestInterface>(a => a.DoSomething(i));
            msg.Invoke(_missing);
        }
    }
}