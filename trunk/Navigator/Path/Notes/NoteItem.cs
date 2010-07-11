using System.Linq;
using System.Xml.Linq;
using Navigator.UI.Attributes;

namespace Navigator.Path.Notes
{
    public class NoteItem : ISummaryString, IDescriptionString
    {
        private readonly XElement _noteElement;

        public NoteItem(XElement noteElement)
        {
            _noteElement = noteElement;
        }

        public string Summary
        {
            get { return _noteElement.Attributes("summary").Single().Value; }
        }

        public string Description
        {
            get { return _noteElement.Value; }
        }
    }
}