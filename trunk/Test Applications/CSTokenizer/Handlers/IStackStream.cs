using System.Collections.Generic;

namespace CSTokenizer
{
    public interface IStackStream<TItemType>
    {
        void Process(char c);
        int Count();
        TItemType Pop();
        void Flush();
    }

    public static class StackStreamer
    {
        public static IEnumerable<TItemType> Stream<TItemType>(IStackStream<TItemType> stream)
        {
            while (stream.Count() > 0)
                yield return stream.Pop();
        }
    }
}