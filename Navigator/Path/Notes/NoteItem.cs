using System.Linq;
using System.Xml.Linq;
using Navigator.UI.Attributes;
using VIControls.Commands;
using VIControls.Commands.Interfaces;

namespace Navigator.Path.Notes
{
    public class NoteItem : ISummaryString, IDescriptionString, IMessageable
    {
        private readonly MessageBroadcaster _messageBroadcaster;
        private readonly XElement _noteElement;
        private readonly EditableText _text;

        public NoteItem(MessageBroadcaster messageBroadcaster, XElement noteElement)
        {
            _messageBroadcaster = messageBroadcaster;
            _noteElement = noteElement;
            _text = new EditableText(_noteElement.Value);
        }

        public string Summary
        {
            get { return _noteElement.Attributes("summary").Single().Value; }
        }

        public string Description
        {
            get { return _text.Text; }
        }

        public object Execute(Message message)
        {
            var ret = message.Invoke(_text);
            _messageBroadcaster.Update();
            return ret;
        }

        public bool CanHandle(Message message)
        {
            return message.CanHandle(_text);
        }
    }
}