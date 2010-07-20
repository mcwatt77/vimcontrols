using System.Linq;
using System.Xml.Linq;
using Navigator.UI.Attributes;

namespace Navigator.Path.Notes
{
    public class NoteItem : ISummaryString, IDescriptionString, IMessageable
    {
        private readonly XElement _noteElement;
        private readonly EditableText _text;

        public NoteItem(XElement noteElement)
        {
            _noteElement = noteElement;
            _text = new EditableText(_noteElement.Value);
        }

        public string Summary
        {
            get { return _noteElement.Attributes("summary").Single().Value; }
        }

        public string Description
        {
            get { return _noteElement.Value; }
        }

        public object Execute(Message message)
        {
            return message.Invoke(_text);
        }

        public bool CanHandle(Message message)
        {
            return message.CanHandle(_text);
        }
    }
}