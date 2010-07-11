using System;
using System.Collections.Generic;
using Navigator.UI.Attributes;

namespace Navigator.Path.Notes
{
    public class SchemaCollection : ISummaryString, IModelChildren
    {
        public string Summary
        {
            get { return "Schemas"; }
        }

        public IEnumerable<object> Children
        {
            get { throw new NotImplementedException(); }
        }
    }
}