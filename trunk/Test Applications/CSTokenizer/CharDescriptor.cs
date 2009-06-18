using System.Collections.Generic;
using System.Linq;

namespace CSTokenizer
{
    public class CharDescriptor
    {
        private CharDescriptor(IEnumerable<string> strings)
        {
            _strings = strings;
        }

        private readonly IEnumerable<string> _strings;

        public IEnumerable<string> GetStrings()
        {
            return _strings;
        }

        public static CharDescriptor FromRange(string range)
        {
            return new CharDescriptor(range.Select(c => c.ToString()));
        }

        public static CharDescriptor FromStrings(params string[] strings)
        {
            return new CharDescriptor(strings);
        }
    }
}