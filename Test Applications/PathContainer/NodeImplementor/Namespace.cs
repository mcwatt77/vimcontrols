using System.Linq;
using Utility.Core;

namespace PathContainer.NodeImplementor
{
    public class Namespace
    {
        private string[] _names;

        public static Namespace Get(string name)
        {
            var ns = new Namespace {_names = new[] {name}};
            return ns;
        }

        public Namespace Combine(Namespace nameSpace)
        {
            return new Namespace {_names = _names.Concat(nameSpace._names).ToArray()};
        }

        public Namespace Combine(string name)
        {
            return new Namespace {_names = _names.Concat(new[] {name}).ToArray()};
        }

        public override string ToString()
        {
            return _names.SeparateBy(".");
        }
    }
}