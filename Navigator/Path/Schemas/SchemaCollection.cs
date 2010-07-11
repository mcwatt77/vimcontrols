using System.Collections.Generic;
using Navigator.UI.Attributes;

namespace Navigator.Path.Schemas
{
    public class SchemaCollection : ISummaryString, IModelChildren
    {
        public string Summary
        {
            get { return "Schemas"; }
        }

        public IEnumerable<object> Children
        {
            get { return new object[]{}; }
        }
    }
}