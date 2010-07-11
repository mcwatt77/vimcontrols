using System.Collections.Generic;
using Navigator.UI.Attributes;

namespace Navigator.Path
{
    public class PathCollection : object, ISummaryString, IModelChildren
    {
        private readonly IEnumerable<object> _elements;

        public PathCollection(params object[] elements)
        {
            _elements = elements;
        }

        public IEnumerable<object> Children
        {
            get { return _elements; }
        }

        public string Summary
        {
            get { return "Navigator"; }
        }
    }
}